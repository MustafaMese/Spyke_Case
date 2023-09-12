using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMachineCase.Component
{
    public class RoomUI : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Sprite blurSprite;
        [SerializeField] private Sprite normalSprite;
        
        private Sprite _currentSprite;
        
        public int PathIndex { get; set; }
        
        public int PathLength { get; set; }

        public void ConfigurePathIndex(int maxRoom)
        {
            PathIndex = (PathIndex + PathLength) % maxRoom;
        }
        
        private void MakeImageTransparent()
        {
            ChangeAlpha(0f);
        }
        
        private void MakeImageOpaque()
        {
            ChangeAlpha(1f);
        }

        private void ChangeAlpha(float value)
        {
            var color = image.color;
            color.a = value;
            image.color = color;
        }

        public void CheckPositionForOpacity(List<Vector3> lastPointsAtPaths)
        {
            var pos = transform.position;

            for (int i = 0; i < lastPointsAtPaths.Count; i++)
            {
                if (Vector3.Distance(pos, lastPointsAtPaths[i]) < 1f)
                {
                    MakeImageTransparent();
                    return;
                }
            }
            
            MakeImageOpaque();
        }

        public void TurnBlur()
        {
            image.sprite = blurSprite;
        }
        
        public void TurnNormal()
        {
            image.sprite = normalSprite;
        }
    }
}