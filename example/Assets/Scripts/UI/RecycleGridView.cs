using System;
using EasingCore;
using FancyScrollView;
using UnityEngine;

namespace UI
{
    public class RecycleGridView : FancyGridView<RecycleData, RecycleContext>
    {
        protected class CellGroup : DefaultCellGroup
        {
            //(*) when group is invisible, unload unused assets and then release memory
            private void OnDisable()
            {
                Resources.UnloadUnusedAssets();
                Debug.Log("RecycleGridView group released memory");
            }
        }

        [SerializeField] GameObject cellPrefab;

        protected override void SetupCellTemplate() => Setup<CellGroup>(cellPrefab);

        public float PaddingTop
        {
            get => paddingHead;
            set
            {
                paddingHead = value;
                Relayout();
            }
        }

        public float PaddingBottom
        {
            get => paddingTail;
            set
            {
                paddingTail = value;
                Relayout();
            }
        }

        public float SpacingY
        {
            get => spacing;
            set
            {
                spacing = value;
                Relayout();
            }
        }

        public float SpacingX
        {
            get => startAxisSpacing;
            set
            {
                startAxisSpacing = value;
                Relayout();
            }
        }

        public void UpdateSelection(int index)
        {
            if (Context.SelectedIndex == index)
            {
                return;
            }

            Context.SelectedIndex = index;
            Refresh();
        }

        public void OnCellClicked(Action<int> callback)
        {
            Context.OnCellClicked = callback;
        }

        public void ScrollTo(int index, float duration, Ease easing, Alignment alignment = Alignment.Middle)
        {
            UpdateSelection(index);
            ScrollTo(index, duration, easing, GetAlignment(alignment));
        }

        public void JumpTo(int index, Alignment alignment = Alignment.Middle)
        {
            UpdateSelection(index);
            JumpTo(index, GetAlignment(alignment));
        }

        public float GetAlignment(Alignment alignment)
        {
            return alignment switch
            {
                Alignment.Upper => 0.0f,
                Alignment.Middle => 0.5f,
                Alignment.Lower => 1.0f,
                _ => GetAlignment(Alignment.Middle)
            };
        }

        protected override void OnScrollerValueChanged(float p)
        {
            base.OnScrollerValueChanged(p);
            if (p>0 && DataCount > startAxisCellCount && p > DataCount / startAxisCellCount)
            {
                _onEndDrag?.Invoke();
                Debug.Log("onEndDrag");
            }
        }
    
        private Action _onEndDrag;
        
        public void OnEndDrag(Action callback)
        {
            _onEndDrag = callback;
        }
    }
}
