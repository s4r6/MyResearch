using System;
using UnityEngine;

namespace Domain.Game
{
    public enum GamePhase
    {
        Moving,
        Inspecting,
        Carrying,
        SelectAction,
        Settings,
        Result,
    }
    public readonly struct GameState : IEquatable<GameState>
    {
        public GamePhase Phase { get; }

        public GameState(GamePhase phase)
        {
            Phase = phase;
        }

        public bool IsMoving => Phase == GamePhase.Moving;
        public bool IsInspecting => Phase == GamePhase.Inspecting;
        public bool IsCarrying => Phase == GamePhase.Carrying;
        public bool IsSelectingAction => Phase == GamePhase.SelectAction;
        public bool IsSettings => Phase == GamePhase.Settings;
        public bool IsResult => Phase == GamePhase.Result;

        public GameState WithPhase(GamePhase newPhase) => new GameState(newPhase);

        // Equals��GetHashCode�̎���
        public override bool Equals(object obj) => obj is GameState other && Equals(other);

        public bool Equals(GameState other) => Phase == other.Phase;

        public override int GetHashCode() => Phase.GetHashCode();

        public static bool operator ==(GameState left, GameState right) => left.Equals(right);
        public static bool operator !=(GameState left, GameState right) => !(left == right);
    }
}

