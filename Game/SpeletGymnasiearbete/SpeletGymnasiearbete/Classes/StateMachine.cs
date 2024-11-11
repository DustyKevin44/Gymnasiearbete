using System.Linq.Expressions;
using Microsoft.Xna.Framework;

namespace SpeletGymnasiearbete.Classes;


 public abstract class State{
    public void Update(GameTime gmt){

    }
    public void EnterState(State newState){
        
       //Set this State to newState

    }
    public void ExitState(){
        
    }
}
public class StateMachine
{
    // The current state of the player
    public State CurrentState { get; private set; }

    // Constructor to initialize with a starting state
    public StateMachine(State initialState)
    {
        CurrentState = initialState;
    }

    // Method to change the state
    public void ChangeState(State newState)
    {
        // Transition from one state to another
        if (CurrentState != newState)
        {
            CurrentState.ExitState();
            CurrentState.EnterState(newState); // Handle logic when entering a new state
            CurrentState = newState;
        }
    }
}
