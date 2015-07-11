using Ninject.Modules;

namespace DigitsRecognition
{
    public class DigitsRecognitionModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IClassifier>().To<BasicClassifier>().InSingletonScope();
            Bind<IDistance>().To<ManhattanDistance>();
            Bind<IEvaluator>().To<Evaluator>();
            Bind<IDataPointsReader>().To<DataPointsReader>();
            Bind<Recognizer>().ToSelf();
        }
    }
}