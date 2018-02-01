using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaireonFramework
{
    public class HeartElement : MonoBehaviour
    {
        enum State { Current = 0, Deflate, Inflate }
        State state, previousState;

        public BeatingHealthBar healthBar;
        public int index;
        public TransitionalObject transition;

        [HideInInspector]
        public new Transform transform;

        void Start()
        {
            transform = base.transform;
        }

        public void UpdateAnimation(float currentPosition)
        {
            previousState = state;

            if(index == currentPosition)
                state = State.Deflate;
            else if(index + 1 < currentPosition)
                state = State.Inflate;
            else if(index > currentPosition)
                state = State.Deflate;
            else
                state = State.Current;

            if(state != previousState)
                switch(state)
                {
                    case State.Inflate://now we have to inflate
                        healthBar.UpdateEndPoint(index);//show we need to inflate
                        break;

                    case State.Deflate:
                        healthBar.UpdateStartPoint(index);//show we need to deflate
                        break;

                    case State.Current:
                        if(previousState == State.Deflate)
                            healthBar.UpdateEndPoint(index);

                        else// if(previousState == State.Inflate)
                            healthBar.UpdateStartPoint(index);
                        break;
                }
        }

        /// <summary>
        /// This is called when the heart has shrunk to the smallest size, it is used to determine how big to grow
        /// </summary>
        public void AtSmallestSize()
        {
            float difference = healthBar.currentValue - index;//has the icon we need to animate changed

            if(difference < 0)//if health needs to decrease
            {
                if(transition.ScalingTransition.transform.localScale == Vector3.zero)//if called again and shrunk to nothing
                    transition.ScalingTransition.endPoint = Vector3.zero;//stop the animation
                else//still mid beat! So shrink to nothing
                {
                    transition.ScalingTransition.startPoint = Vector3.zero;//shrink to nothing
                    transition.ScalingTransition.endPoint = transition.ScalingTransition.transform.localScale;//start at currentSize
                    transition.TriggerFadeOut();
                }
            }
            #region Beat Normally
            else if(difference < 1)
                healthBar.UpdateEndPoint(index);
            #endregion
        }

        /// <summary>
        /// This is called when the heart has grown to its largest size, it is used to determine how small to shrink
        /// </summary>
        public void AtBiggestSize()
        {
            float difference = healthBar.currentValue - index;//has the icon we need to animate changed

            if(difference >= 1)//if health needs to increase
            {
                if(transition.ScalingTransition.transform.localScale == Vector3.one)//if called again and full size
                    transition.ScalingTransition.startPoint = Vector3.one;//basically stop the animation
                else//still mid beat! So expand fully
                {
                    transition.ScalingTransition.endPoint = Vector3.one;//expand fully
                    transition.ScalingTransition.startPoint = transition.ScalingTransition.transform.localScale;//start at currentSize
                    transition.TriggerTransition();
                }
            }
            #region Beat Normally
            else if(difference > 0)
                healthBar.UpdateStartPoint(index);
            #endregion
        }
    }
}