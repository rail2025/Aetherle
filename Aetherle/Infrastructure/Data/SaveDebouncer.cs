using System;
using System.Timers;

namespace Aetherle.Infrastructure.Data;

public class SaveDebouncer
{
    private readonly Action saveAction;
    private readonly Timer timer;

    public SaveDebouncer(Action saveAction, int delayMilliseconds = 1000)
    {
        this.saveAction = saveAction ?? throw new ArgumentNullException(nameof(saveAction));
        this.timer = new Timer(delayMilliseconds) { AutoReset = false };
        this.timer.Elapsed += (s, e) => this.saveAction();
    }

    public void Trigger()
    {
        this.timer.Stop();
        this.timer.Start();
    }
}
