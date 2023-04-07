using System;
using WPFClient.Model;

namespace WPFClient.Store
{
    public class ProfileStore
    {
        private PlayerModel? _playerModel;
        public PlayerModel PlayerModel => _playerModel ?? throw new NullReferenceException(); 

    }
}
