using System.Collections;
using System.Collections.Generic;


public interface IState
{
    void Enter(float dT);
    void FixedUpdate(float dT);
    void Update(float dT);
    void Exit(float dT);
}
