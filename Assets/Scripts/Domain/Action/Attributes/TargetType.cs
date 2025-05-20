namespace Domain.Action
{
    public enum TargetType
    {
        Self,      // アクション対象のエンティティ（例: PC）
        HeldItem   // プレイヤーが手に持っているエンティティ（例: Memo）
    }
}