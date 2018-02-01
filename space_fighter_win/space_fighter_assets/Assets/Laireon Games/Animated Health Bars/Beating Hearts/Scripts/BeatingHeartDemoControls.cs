using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LaireonFramework
{
    public class BeatingHeartDemoControls : MonoBehaviour
    {
        public Slider valueSlider;
        public Text iconCountText;

        public BeatingHealthBar[] healthBars;

        void Start()
        {
            valueSlider.maxValue = healthBars[0].maxValue;
            iconCountText.text = healthBars[0].MaxIcons + "";
        }

        void Update()
        {
            for(int i = 0; i < healthBars.Length; i++)
                healthBars[i].currentValue = valueSlider.value;
        }

        public void HitPlus()
        {
            if(healthBars[0].MaxIcons < 10)
            {
                for(int i = 0; i < healthBars.Length; i++)
                {
                    healthBars[i].MaxIcons++;
                    healthBars[i].maxValue++;
                }

                valueSlider.maxValue = healthBars[0].maxValue;
                iconCountText.text = healthBars[0].MaxIcons + "";
            }
        }

        public void HitMinus()
        {
            if(healthBars[0].MaxIcons > 5)
            {
                for(int i = 0; i < healthBars.Length; i++)
                {
                    healthBars[i].MaxIcons--;
                    healthBars[i].maxValue--;
                }

                valueSlider.maxValue = healthBars[0].maxValue;
                iconCountText.text = healthBars[0].MaxIcons + "";
            }
        }
    }
}
