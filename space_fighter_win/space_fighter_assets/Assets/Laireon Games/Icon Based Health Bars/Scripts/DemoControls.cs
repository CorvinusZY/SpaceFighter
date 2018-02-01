using UnityEngine;
using System.Collections;

public class DemoControls : MonoBehaviour
{
    public IconProgressBar healthBar, armourBar;

    void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.Width(250));
        GUILayout.Space(25);

        GUILayout.BeginHorizontal();
        GUILayout.Space(25);

        GUILayout.Label("Health");
        healthBar.currentValue = GUILayout.HorizontalSlider(healthBar.currentValue, 0, healthBar.maxValue);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(25);

        GUILayout.Label("Armour");
        armourBar.currentValue = GUILayout.HorizontalSlider(armourBar.currentValue, 0, armourBar.maxValue);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(25);
        GUILayout.Label("Health Whole Numbers");
        healthBar.showFractions = !GUILayout.Toggle(!healthBar.showFractions, "");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(25);
        GUILayout.Label("Armour Whole Numbers");
        armourBar.showFractions = !GUILayout.Toggle(!armourBar.showFractions, "");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(25);

        GUILayout.Label("Health Icons");
        healthBar.MaxIcons = (int)GUILayout.HorizontalSlider(healthBar.MaxIcons, 5, 10);
        healthBar.maxValue = healthBar.MaxIcons;

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(25);

        GUILayout.Label("Armour Icons");
        armourBar.MaxIcons = (int)GUILayout.HorizontalSlider(armourBar.MaxIcons, 5, 10);
        armourBar.maxValue = armourBar.MaxIcons;

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }
}
