using Game.Scripts.Characters.CharacterResources.DefaultResources;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.BattleActionsScreen.View
{
    public class BattleResourcesItemView : MonoBehaviour
    {
        [field: SerializeField] public Image[] Icons { get; private set; }

        [HideInInspector] public CharacterResourceData Target;
        [HideInInspector] public int MarkLevel;

        public void SetData(
            Sprite sprite,
            int count,
            int maxCount,
            Color color)
        {
            int length = Mathf.Min(count, Icons.Length);
            int i = 0;
            for (i = 0; i < length; i++)
            {
                Icons[i].sprite = sprite;
                Icons[i].color = color;
                Icons[i].gameObject.SetActive(true);
            }
            
            length = Mathf.Min(maxCount, Icons.Length);
            var darkColor = color / 3;
            for (; i < length; i++)
            {
                Icons[i].sprite = sprite;
                Icons[i].color = darkColor;
                Icons[i].gameObject.SetActive(true);
            }

            for (; i < Icons.Length; i++)
            {
                Icons[i].gameObject.SetActive(false);
            }
        }
    }
}