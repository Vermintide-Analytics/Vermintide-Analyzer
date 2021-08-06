using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VA.LogReader;

using FS = System.IO.FileStream;

namespace LogGenerator
{
    class Program
    {
        static string outputDir = Path.Combine(Environment.CurrentDirectory, "GENERATED");

        static Random rand = new Random();

        static int TIMESTAMP_BYTES = 2;
        static int PAYLOAD_BYTES = VA.LogReader.Event.BYTES - TIMESTAMP_BYTES;
        static byte[] dataBytes = new byte[PAYLOAD_BYTES];

        static int PAYLOAD_BITS = PAYLOAD_BYTES * 8;

        static Func<ROUND_RESULT> resultGenerator;

        static long lastTime = 0;

        static List<Tuple<float, Action<FS>>> LogGenerators = new List<Tuple<float, Action<FS>>>()
        {
            { new Tuple<float, Action<FS>>(85, WriteNonCritDamageDealt) },
            { new Tuple<float, Action<FS>>(15, WriteCritDamageDealt) },
            { new Tuple<float, Action<FS>>(35, WriteTempHPGained) },
            { new Tuple<float, Action<FS>>(5, WriteDamageTaken) },
            { new Tuple<float, Action<FS>>(40, WriteEnemyKilled) },
            //{ new Tuple<float, Action<FS>>(60, WriteEnemyStaggered) },
            { new Tuple<float, Action<FS>>(20, WriteCurrentHealth) },
            { new Tuple<float, Action<FS>>(0.15f, WritePlayerState) },
        };
        static float WeightTotal = LogGenerators.Sum(tuple => tuple.Item1);
        static Action<FS> GetRandomLogGenerator()
        {
            var randNum = rand.NextDouble() * WeightTotal;
            var index = 0;
            do
            {
                randNum -= LogGenerators[index].Item1;
                index++;
            } while (randNum > 0 && index < LogGenerators.Count);

            return LogGenerators[index-1].Item2;
        }

        static void Main(string[] args)
        {
            if(!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            foreach(var path in Directory.GetFiles(outputDir, "*.VA"))
            {
                new FileInfo(path).Delete();
            }

            Console.WriteLine("Desired # of files:");
            var numFiles = int.Parse(Console.ReadLine());
            Console.WriteLine("Desired # of events:");
            var numEvents = int.Parse(Console.ReadLine());
            Console.WriteLine("Desired round result (Win, Loss, Quit, leave empty for random):");
            var resultInput = Console.ReadLine();
            if(resultInput.ToLower() == "random" || resultInput == "")
            {
                resultGenerator = RandomResult;
            }
            else
            {
                var result = (ROUND_RESULT)Enum.Parse(typeof(ROUND_RESULT), resultInput);
                resultGenerator = () => result;
            }

            Console.WriteLine("Omit talent data?");
            var omitTalentsInput = Console.ReadLine();
            Console.WriteLine("Omit weapon data?");
            var omitWeaponsInput = Console.ReadLine();
            var omitTalents = omitTalentsInput.ToLower() == "true" || omitTalentsInput == "yes" || omitTalentsInput == "y" || omitTalentsInput == "t";
            var omitWeapons = omitWeaponsInput.ToLower() == "true" || omitWeaponsInput == "yes" || omitWeaponsInput == "y" || omitWeaponsInput == "t";

            for (int fileNum = 0; fileNum < numFiles; fileNum++)
            {
                var fakeTime = DateTime.Now - TimeSpan.FromDays(fileNum);
                
                lastTime = 0;

                string fileName = $"{fakeTime.ToString(Game.LOG_DATE_TIME_FORMAT)}.VA";
                string filePath = Path.Combine(outputDir, fileName);

                using (var s = File.OpenWrite(filePath))
                {
                    // Write header
                    s.WriteByte(Schema.SCHEMA_VERSION_MAJOR);
                    s.WriteByte(Schema.SCHEMA_VERSION_MINOR);
                    s.WriteByte(RandomByte()); // Game version major
                    s.WriteByte(RandomByte()); // Game version minor
                    s.WriteByte(RandomByte(1)); // Deathwish enabled
                    s.WriteByte(RandomByte(4)); // Onslaught type
                    s.WriteByte(RandomByte(1)); // Empowered enabled

                    // Write initial expected events
                    var (diff, car, camp) = WriteRoundStart(s);
                    if(!omitWeapons)
                    {
                        WriteWeaponSet(s);
                    }
                    if(!omitTalents)
                    {
                        WriteTalentTree(s);
                    }

                    // Write requested number of random events
                    for(int count = 0; count < numEvents; count++)
                    {
                        GetRandomLogGenerator().Invoke(s);
                        //LogGenerators[rand.Next(1, LogGenerators.Count + 1)].Invoke(s);
                    }

                    // Write end event
                    WriteRoundEnd(s);
                }
            }
        }

        static (DIFFICULTY, CAREER, CAMPAIGN) WriteRoundStart(FS s)
        {
            var diff = RandomDifficulty();
            var car = RandomCareer();
            var mission = RandomMission();
            var camp = mission.Campaign();

            var missionShift = camp.MissionEnumShift();

            long data = Event(EventType.Round_Start);
            data += (long)diff << Bitshift.DIFFICULTY;
            data += (long)car << Bitshift.CAREER;
            data += (long)camp << Bitshift.CAMPAIGN;
            data += (((long)mission >> missionShift) - 1) << Bitshift.MISSION;

            WriteLog(s, data);
            return (diff, car, camp);
        }

        static void WriteWeaponSet(FS s)
        {
            long data = Event(EventType.Weapon_Set);

            data += RandomMeleeWeapon() << Bitshift.WEAPON1;
            data += RandomRangedWeapon() << Bitshift.WEAPON2;

            WriteLog(s, data);
        }

        static void WriteTalentTree(FS s)
        {
            long data = Event(EventType.Talent_Tree);
            data += rand.Next() & Bitmask.TALENTS;

            WriteLog(s, data);
        }

        static void WriteDamageDealt(FS s, bool crit)
        {
            long data = Event(EventType.Damage_Dealt);

            long critVal = crit ? 1 : 0;
            data += critVal << Bitshift.CRIT;
            data += (long)RandomTarget() << Bitshift.TARGET;
            data += (long)RandomSource() << Bitshift.DAMAGE_SOURCE;

            data += rand.Next(crit ? 100 : 0, crit ? 255 : 150) & Bitmask.DAMAGE_INT;
            data += rand.Next() & Bitmask.DAMAGE_FRACTION;

            WriteLog(s, data);
        }

        static void WriteNonCritDamageDealt(FS s) => WriteDamageDealt(s, false);
        static void WriteCritDamageDealt(FS s) => WriteDamageDealt(s, true);

        static void WriteTempHPGained(FS s)
        {
            long data = Event(EventType.Temp_HP_Gained);

            int uncapped_heal = rand.Next(0, 100);
            data += (uncapped_heal << Bitshift.UNCAPPED_HEAL_INT) & Bitmask.UNCAPPED_HEAL_INT;
            data += (rand.Next() << Bitshift.UNCAPPED_HEAL_FRACTION) & Bitmask.UNCAPPED_HEAL_FRACTION;
            data += (rand.Next(0, uncapped_heal+1) << Bitshift.CAPPED_HEAL_INT) & Bitmask.CAPPED_HEAL_INT;
            data += (rand.Next() << Bitshift.CAPPED_HEAL_FRACTION) & Bitmask.CAPPED_HEAL_FRACTION;

            WriteLog(s, data);
        }

        static void WriteDamageTaken(FS s)
        {
            long data = Event(EventType.Damage_Taken);
            data += rand.Next(0, 120) & Bitmask.DAMAGE_INT;
            data += rand.Next() & Bitmask.DAMAGE_FRACTION;

            WriteLog(s, data);
        }

        static void WriteCurrentHealth(FS s)
        {
            long data = Event(EventType.Current_Health);

            var total = rand.Next(0, 150);
            var permanent = total - rand.Next(0, total);
            var temp = total - permanent;

            data += (permanent << Bitshift.PERMANENT_HEALTH) & Bitmask.PERMANENT_HEALTH ;
            data += temp & Bitmask.TEMPORARY_HEALTH;

            WriteLog(s, data);
        }

        static void WriteEnemyKilled(FS s)
        {
            long data = Event(EventType.Enemy_Killed);

            data += (long)RandomEnemyType() << Bitshift.ENEMY_TYPE;
            data += (long)RandomSource() << Bitshift.DAMAGE_SOURCE;

            data += rand.Next(0, 255) & Bitmask.DAMAGE_INT;
            data += rand.Next() & Bitmask.DAMAGE_FRACTION;

            WriteLog(s, data);
        }

        static void WriteEnemyStaggered(FS s)
        {
            long data = Event(EventType.Enemy_Staggered);

            data += rand.Next(1, 8) << Bitshift.STAGGER_LEVEL;
            data += (long)RandomSource() << Bitshift.STAGGER_SOURCE;

            data += rand.Next(0, 6000) & Bitmask.STAGGER_DURATION;

            WriteLog(s, data);
        }

        static void WritePlayerState(FS s)
        {
            long data = Event(EventType.Player_State);

            data += (long)RandomPlayerState();

            WriteLog(s, data);
        }

        static void WriteRoundEnd(FS s)
        {
            long data = Event(EventType.Round_End);
            data += (long)resultGenerator();

            WriteLog(s, data);
        }

        static void WriteTimestamp(FS s)
        {
            lastTime += rand.Next(0, 50);
            byte b2 = (byte)lastTime;
            byte b1 = (byte)(lastTime >> 8);
            s.WriteByte(b1);
            s.WriteByte(b2);
        }


        static Array CareerVals = Enum.GetValues(typeof(CAREER));
        static CAREER RandomCareer()
        {
            return (CAREER)CareerVals.GetValue(rand.Next(CareerVals.Length));
        }

        static Array DifficultyVals = Enum.GetValues(typeof(DIFFICULTY));
        static DIFFICULTY RandomDifficulty()
        {
            return (DIFFICULTY)DifficultyVals.GetValue(rand.Next(DifficultyVals.Length));
        }

        static Array CampaignVals = Enum.GetValues(typeof(CAMPAIGN));
        static CAMPAIGN RandomCampaign()
        {
            return (CAMPAIGN)CampaignVals.GetValue(rand.Next(CampaignVals.Length));
        }

        static Array MissionVals = Enum.GetValues(typeof(MISSION)).Cast<MISSION>().Where(v => v != MISSION.Unknown).ToArray();
        static MISSION RandomMission()
        {
            return (MISSION)MissionVals.GetValue(rand.Next(MissionVals.Length));
        }

        static Array ResultVals = Enum.GetValues(typeof(ROUND_RESULT));
        static ROUND_RESULT RandomResult()
        {
            return (ROUND_RESULT)ResultVals.GetValue(rand.Next(ResultVals.Length));
        }

        static Array TargetVals = Enum.GetValues(typeof(DAMAGE_TARGET));
        static DAMAGE_TARGET RandomTarget()
        {
            return (DAMAGE_TARGET)TargetVals.GetValue(rand.Next(TargetVals.Length));
        }

        static Array SourceVals = Enum.GetValues(typeof(DAMAGE_SOURCE));
        static DAMAGE_SOURCE RandomSource()
        {
            return (DAMAGE_SOURCE)SourceVals.GetValue(rand.Next(SourceVals.Length));
        }

        static Array EnemyTypeVals = Enum.GetValues(typeof(ENEMY_TYPE));
        static ENEMY_TYPE RandomEnemyType()
        {
            return (ENEMY_TYPE)EnemyTypeVals.GetValue(rand.Next(EnemyTypeVals.Length));
        }

        static Array PlayerStateVals = Enum.GetValues(typeof(PLAYER_STATE));
        static PLAYER_STATE RandomPlayerState()
        {
            return (PLAYER_STATE)PlayerStateVals.GetValue(rand.Next(PlayerStateVals.Length));
        }

        static long RandomMeleeWeapon()
        {
            return rand.Next(0, 6);
        }

        static long RandomRangedWeapon()
        {
            return rand.Next(32, 35);
        }

        static byte RandomByte(byte max = 255) => (byte)rand.Next(0, max + 1);

        static long Event(EventType type) => ((long)type << Bitshift.EVENT_TYPE) << (PAYLOAD_BITS - 8);

        static void WriteLog(FS s, long data)
        {
            WriteTimestamp(s);
            WriteData(s, data);
        }

        static void WriteData(FS s, long data)
        {
            SetDataBytes(data);
            WriteDataBytes(s);
        }

        static void SetDataBytes(long val)
        {
            for (int count = 0; count < PAYLOAD_BYTES; count++)
            {
                dataBytes[PAYLOAD_BYTES - count - 1] = (byte)(val >> (count * 8));
            }
        }

        static void WriteDataBytes(FS s)
        {
            for(int count = 0; count < PAYLOAD_BYTES; count++)
            {
                s.WriteByte(dataBytes[count]);
            }
        }
    }
}
