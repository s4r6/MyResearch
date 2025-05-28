using UnityEngine;

namespace Domain.Component
{
    public class PcComponent : GameComponent
    {
        public string RequiredUsername { get; }
        public string RequiredPassword { get; }

        public bool IsLoggedIn { get; private set; }
        public int FailedAttempts { get; private set; }

        public bool IsPoweredOn { get; private set; }

        public PcComponent(string requiredUsername = "admin", string requiredPassword = "c92Y3ki7t")
        {
            RequiredUsername = requiredUsername;
            RequiredPassword = requiredPassword;
            IsPoweredOn = true;
        }

        public bool TryLogin(string inputUser, string inputPass)
        {
            if (!IsPoweredOn) return false;

            if (inputUser == RequiredUsername && inputPass == RequiredPassword)
            {
                IsLoggedIn = true;
                FailedAttempts = 0;
                return true;
            }
            else
            {
                FailedAttempts++;
                return false;
            }
        }

        public void Shutdown()
        {
            IsPoweredOn = false;
            IsLoggedIn = false;
        }

        public void Boot()
        {
            IsPoweredOn = true;
        }
    }
}