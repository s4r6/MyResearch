using UnityEngine;

namespace Domain.Game
{
    public class DocumentEntity
    {
        public bool IsOpen;
        public DocumentEntity()
        {
        }

        public void Open()
        {
            IsOpen = true;
        }

        public void Close() 
        {
            IsOpen = false;
        }
    }
}