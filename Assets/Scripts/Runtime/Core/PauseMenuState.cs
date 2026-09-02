using UnityEngine;

namespace FlowState.Runtime.Core
{
    public sealed class PauseMenuState
    {
        private const float NavigationThreshold = 0.5f;

        public bool IsActive { get; private set; }

        public E_PauseMenuSelection CurrentSelection { get; private set; } =
            E_PauseMenuSelection.Resume;

        public bool Activate()
        {
            if (IsActive)
            {
                return false;
            }

            CurrentSelection = E_PauseMenuSelection.Resume;
            IsActive = true;
            return true;
        }

        public bool Deactivate()
        {
            if (!IsActive)
            {
                return false;
            }

            IsActive = false;
            CurrentSelection = E_PauseMenuSelection.Resume;
            return true;
        }

        public bool TryMove(float verticalInput)
        {
            if (!IsActive || Mathf.Abs(verticalInput) < NavigationThreshold)
            {
                return false;
            }

            int direction = verticalInput < 0.0f ? 1 : -1;
            int nextSelection = (int)CurrentSelection + direction;

            if (nextSelection < (int)E_PauseMenuSelection.Resume ||
                nextSelection > (int)E_PauseMenuSelection.Quit)
            {
                return false;
            }

            CurrentSelection = (E_PauseMenuSelection)nextSelection;
            return true;
        }

        public bool TrySelectAtPointer(E_PauseMenuSelection selection)
        {
            if (!IsActive || !IsValidSelection(selection))
            {
                return false;
            }

            CurrentSelection = selection;
            return true;
        }

        public bool TrySubmit(out E_PauseMenuSelection selection)
        {
            selection = CurrentSelection;
            return IsActive;
        }

        public bool TryCancel(out E_PauseMenuSelection selection)
        {
            selection = E_PauseMenuSelection.Resume;
            return IsActive;
        }

        public bool TryClick(
            E_PauseMenuSelection selection,
            out E_PauseMenuSelection executedSelection)
        {
            executedSelection = CurrentSelection;

            if (!TrySelectAtPointer(selection))
            {
                return false;
            }

            executedSelection = CurrentSelection;
            return true;
        }

        private static bool IsValidSelection(E_PauseMenuSelection selection)
        {
            return selection >= E_PauseMenuSelection.Resume &&
                   selection <= E_PauseMenuSelection.Quit;
        }
    }
}
