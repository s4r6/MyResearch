using Infrastructure.Stage.Object;

namespace UseCase.Stage
{
    public class StageSystemUseCase
    {
        InspectableObjectRepository inspectableObjectRepository;
        public StageSystemUseCase(InspectableObjectRepository inspectableObjectRepository)
        {
            this.inspectableObjectRepository = inspectableObjectRepository;
        }

        
    }
}
