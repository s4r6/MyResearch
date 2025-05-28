using Domain.Component;
using UnityEngine;
using UseCase.Player;
using View.UI;

namespace UseCase.Stage
{
    public class PCUseCase
    {
        IObjectRepository repository;
        LoginView view;
        public PCUseCase(IObjectRepository repository, LoginView view) 
        { 
            this.repository = repository;
            this.view = view;
        }
        public bool OnPCInteract(string objectId)
        {
            var entity = repository.GetById(objectId);
            if (entity == null || entity.TryGetComponent<PcComponent>(out var pc))
                return false;

            return true;
        }

        public bool TryLogin(string objectId ,string username, string password)
        {

            return true;
        }
    }
}