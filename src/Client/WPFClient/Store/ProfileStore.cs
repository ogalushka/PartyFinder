﻿using System;
using System.Collections.Generic;
using WPFClient.Model;

namespace WPFClient.Store
{
    public class ProfileStore
    {
        private PlayerModel _playerModel = new PlayerModel("", new List<GameModel>(), new List<TimeRangeModel>());
        public PlayerModel PlayerModel => _playerModel; 

    }
}
