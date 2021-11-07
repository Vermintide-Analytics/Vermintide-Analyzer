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

        static long lastTime = 0;

        //static List<Tuple<float, Action<FS>>> LogGenerators = new List<Tuple<float, Action<FS>>>()
        //{
        //    { new Tuple<float, Action<FS>>(85, WriteNonCritDamageDealt) },
        //    { new Tuple<float, Action<FS>>(15, WriteCritDamageDealt) },
        //    { new Tuple<float, Action<FS>>(35, WriteTempHPGained) },
        //    { new Tuple<float, Action<FS>>(5, WriteDamageTaken) },
        //    { new Tuple<float, Action<FS>>(40, WriteEnemyKilled) },
        //    //{ new Tuple<float, Action<FS>>(60, WriteEnemyStaggered) },
        //    { new Tuple<float, Action<FS>>(20, WriteCurrentHealth) },
        //    { new Tuple<float, Action<FS>>(0.15f, WritePlayerState) },
        //};
        //static float WeightTotal = LogGenerators.Sum(tuple => tuple.Item1);
        //static Action<FS> GetRandomLogGenerator()
        //{
        //    var randNum = rand.NextDouble() * WeightTotal;
        //    var index = 0;
        //    do
        //    {
        //        randNum -= LogGenerators[index].Item1;
        //        index++;
        //    } while (randNum > 0 && index < LogGenerators.Count);

        //    return LogGenerators[index - 1].Item2;
        //}

        static void Main(string[] args)
        {
           // TODO: Bring this back after the 2.0 schema update
        }
    }
}
