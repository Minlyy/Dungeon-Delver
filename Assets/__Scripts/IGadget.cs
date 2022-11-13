public interface IGadget
{
    // a
    bool GadgetUse(Dray tDray, System.Func<IGadget, bool> tDoneCallback); // b
    bool GadgetCancel();
    // c
    string name { get; }
    // d
}

