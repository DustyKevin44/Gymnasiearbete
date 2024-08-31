using Microsoft.Xna.Framework;
using static SpeletGymnasiearbete.Utils;

namespace SpeletGymnasiearbete.Classes;

public class Timer(float duration, bool repeat) : IGameObject
{
    public bool Object_is_dying { get; private set; } = false;
    public float Duration { get; } = duration;
    public bool Repeat { get; } = repeat;

    public float ElapsedTime { get; private set; } = 0f;
    public bool Finished { get; private set; } = false;
    public bool Paused { get; private set; } = true;

    // Start the timer, if it doesn't repeat reset it
    public void StartTimer()
    {
        Paused = false;
        if (!Repeat) { ElapsedTime = 0f; }
    }

    // Pause the timer
    public void PauseTimer() { Paused = true; }

    public void Draw() {}
    public void Queue_kill() { Object_is_dying = true; }

    public void Update(GameTime gameTime)
    {
        // If paused don't update
        if (Paused) { return; }
        // Increment the timer
        ElapsedTime += GameTimeToDelta(gameTime);
        // If timer hasn't passed the set time
        if (ElapsedTime < Duration) { Finished = false; return; }
        Finished = true;
        // If repeatin reset the time else pause
        if (Repeat) {
            ElapsedTime -= Duration;
        } else {
            Paused = true;
        }
    }
}
