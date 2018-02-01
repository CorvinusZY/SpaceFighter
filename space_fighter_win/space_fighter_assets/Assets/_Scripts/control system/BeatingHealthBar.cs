using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

namespace LaireonFramework
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteInEditMode]
    public class BeatingHealthBar : IconProgressBar
    {
        public float minHeartSize, maxHeartSize;
        public float minBeatingTime, maxBeatingTime;

        public float yPivot = 0.5f;

        public GameObject heartPrefab;

        public List<HeartElement> hearts = new List<HeartElement>();

        protected override void UpdateAnimation()
        {
            for(int i = 0; i < maxIcons; i++)
            {
                ((RectTransform)backings[i].transform).anchoredPosition = new Vector2(backingImage.rect.width * (i + 0.5f) - ((RectTransform)transform).sizeDelta.x * 0.5f, 0);
                backings[i].transform.position = new Vector3(backings[i].transform.position.x, backings[i].transform.position.y, transform.position.z);
                mainImages[i].transform.position = backings[i].transform.position;

                ((RectTransform)mainImages[i].transform).anchoredPosition -= new Vector2(0, ((RectTransform)mainImages[i].transform).sizeDelta.y * (0.5f - yPivot));//now take into consideration the changed pivot!

                hearts[i].UpdateAnimation(currentValue);
            }
        }

        /// <summary>
        /// Called when expanding
        /// </summary>
        public void UpdateStartPoint(int index)
        {
            float minSize;

            if(index + 1 == currentValue)
                minSize = 1;
            else if(currentValue == index)
                minSize = 0;
            else if(index >= currentValue)
                minSize = 0;
            else
                minSize = currentValue % (maxValue / MaxIcons);//convert to find the percentage of the current heart 

            hearts[index].transition.FirstTransition.transitionInTime = Lerp(maxBeatingTime, minBeatingTime, minSize);
            hearts[index].transition.FirstTransition.fadeOutTime = hearts[index].transition.FirstTransition.transitionInTime;//overcomes a frame delay between this being set

            if(index < currentValue)
                minSize = Mathf.Max(minSize, minHeartSize);//don't go smaller than the min! 

            hearts[index].transition.ScalingTransition.startPoint = new Vector3(minSize, minSize, minSize);
            hearts[index].transition.ScalingTransition.endPoint = hearts[index].transform.localScale;
            hearts[index].transition.TriggerFadeOut();
            hearts[index].transition.ScalingTransition.JumpToEnd(false);

#if(UNITY_EDITOR)
            if(!UnityEditor.EditorApplication.isPlaying)
                hearts[index].transform.localScale = hearts[index].transition.ScalingTransition.startPoint;
#endif
        }

        /// <summary>
        /// Called when shrinking
        /// </summary>
        public void UpdateEndPoint(int index)
        {
            float minSize;

            if(index + 1 <= currentValue)//currentValue == maxValue ||
                minSize = 1;
            else
                minSize = currentValue % (maxValue / MaxIcons);//convert to find the percentage of the current heart 

            float maxSize = Lerp(maxHeartSize, 1, minSize);//use min heart size whilst its still a relative percentage to determine the max size

            if(currentValue == index)
                maxSize = 0;

            hearts[index].transition.FirstTransition.transitionInTime = Lerp(maxBeatingTime, minBeatingTime, minSize);
            hearts[index].transition.FirstTransition.fadeOutTime = hearts[index].transition.FirstTransition.transitionInTime;//overcomes a frame delay between this being set

            hearts[index].transition.ScalingTransition.endPoint = new Vector3(maxSize, maxSize, maxSize);
            hearts[index].transition.ScalingTransition.startPoint = hearts[index].transform.localScale;
            hearts[index].transition.TriggerTransition();
            hearts[index].transition.ScalingTransition.JumpToStart();

#if(UNITY_EDITOR)
            if(!UnityEditor.EditorApplication.isPlaying)
                hearts[index].transform.localScale = hearts[index].transition.ScalingTransition.endPoint;
#endif
        }

        public void UpdatePivot()
        {
            for(int i = 0; i < maxIcons; i++)
                ((RectTransform)mainImages[i].transform).pivot = new Vector2(0.5f, yPivot);
        }

        protected override GameObject InstantiateMainImage(int index)
        {
            GameObject current = Instantiate<GameObject>(heartPrefab);
            current.transform.SetParent(mainImageContainer);
            current.transform.localScale = Vector3.one;
            ((RectTransform)current.transform).sizeDelta = new Vector2(mainImage.rect.width, mainImage.rect.height);
            ((RectTransform)current.transform).pivot = new Vector2(0.5f, yPivot);

            if(index > currentValue)//if after the current position
                current.transform.localScale = Vector3.zero;//hide until told otherwise!

            HeartElement heart = current.GetComponent<HeartElement>();
            heart.transform = current.transform;
            heart.healthBar = this;
            heart.index = index;
            heart.transition.ScalingTransition.transform = current.transform;//affect the heart

            while(hearts.Count <= index)
                hearts.Add(null);

            hearts[index] = heart;

            UpdateStartPoint(index);
            UpdateEndPoint(index);

            return current;
        }

        /// <summary>
        /// Returns the value that is the defined percentage between both values
        /// </summary>
        public static float Lerp(float min, float max, float percentageBetween)
        {
            float differenceSign = (min - max > 0 ? -1 : 1);//if the difference is negative then subtract the value at the end!

            return min + Mathf.Abs(min - max) * percentageBetween * differenceSign;
        }
    }
}