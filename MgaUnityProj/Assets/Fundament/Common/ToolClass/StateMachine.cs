//=============================================================================
// template classes for state design pattern
// see also: Design Pattern:State
//=============================================================================

public class TState<T>
{
    public TState() {}

    public virtual void Enter(T pObj) {}
    public virtual void Execute(T pObj) {}
    public virtual void Update(T pObj, float fDeltaTime) {}
    public virtual void Exit(T pObj) {}
};

public class TStateMachine<T>
{
    protected T m_pOwner;

    protected TState<T> m_pCurrentState;
    protected TState<T> m_pPreviousState;  //a record of the last state the agent was in
    protected TState<T> m_pNewState;

    public TStateMachine()
    {
        m_pCurrentState = null;
        m_pPreviousState = null;
        m_pNewState = null;
    }

    public TStateMachine(T owner)
    { 
        m_pOwner = owner;
        m_pCurrentState = null;
        m_pPreviousState = null;
        m_pNewState = null;
    }

    //use these methods to initialize the FSM
    public void SetCurrentState(TState<T> s){m_pCurrentState = s;}
    public void SetPreviousState(TState<T> s){m_pPreviousState = s;}
 

    //call this to update the FSM
    public void Update(float deltaTime)
    {
        //same for the current state
        if (null != m_pCurrentState)
		{
            m_pCurrentState.Execute(m_pOwner);
		    m_pCurrentState.Update(m_pOwner, deltaTime);
		}
    }

    //change to a new state
    public void ChangeState(TState<T> pNewState)
    {

        //keep a record of the previous state
        m_pPreviousState = m_pCurrentState;
        m_pNewState = pNewState; //for current state exit use

        //call the exit method of the existing state
        if (null != m_pCurrentState)
        {
            m_pCurrentState.Exit(m_pOwner);    
        }

        //change state to the new state
        m_pCurrentState = pNewState;

        //call the entry method of the new state
        m_pCurrentState.Enter(m_pOwner);
    }

    //change state back to the previous state
    public void RevertToPreviousState()
    {
        ChangeState(m_pPreviousState);
    }
 
    //accessors
    public TState<T> CurrentState()  {return m_pCurrentState;}
    public TState<T> PreviousState() {return m_pPreviousState;}
    public TState<T> NextState() { return m_pNewState; }
    public T Owner() {return m_pOwner;}
};
