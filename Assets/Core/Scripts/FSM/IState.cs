﻿using System.Collections;
using System.Collections.Generic;


public interface IState
{
    void Enter();
    void FixedUpdate();
    void Update();
    void Exit();
}
