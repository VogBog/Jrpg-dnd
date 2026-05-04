using Game.Scripts.UI.BattleActionsScreen.ModelView;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.BattleActionsScreen.View
{
    public class ShowActionInfoPanel : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Image[] _resourcesIcons;
        
        public void SetData(ActionInfo info)
        {
            _image.sprite = info.Data.Icon;
            _name.text = info.Data.Name;
            _description.text = info.Data.Description;

            int i = 0;
            foreach (var resource in info.UsingResources)
            {
                if (i >= _resourcesIcons.Length)
                    return;

                _resourcesIcons[i].sprite = resource.Icon;
                _resourcesIcons[i].color = resource.Color;
                _resourcesIcons[i].gameObject.SetActive(true);

                ++i;
            }
            
            foreach (var (resource, mark) in info.UsingMarkedResources)
            {
                if (i >= _resourcesIcons.Length)
                    return;

                _resourcesIcons[i].sprite = resource.Icon;
                _resourcesIcons[i].color = resource.Color;
                _resourcesIcons[i].gameObject.SetActive(true);

                foreach (var variant in resource.Variants)
                {
                    if (variant.MarkValue == mark)
                    {
                        _resourcesIcons[i].sprite = variant.Icon;
                        break;
                    }
                }
                
                _resourcesIcons[i].gameObject.SetActive(true);

                ++i;
            }

            for (; i < _resourcesIcons.Length; ++i)
            {
                _resourcesIcons[i].gameObject.SetActive(false);
            }
        }
    }
}