using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;

public static class HelperFunctions {

    public static bool IsWithin(this float value, float min, float max) {   // 'this float value' refers to the value connected to the method; usage: [value].IsWithin(min, max);
        return value >= min && value <= max;
    }

    public static void IncrementInt(string key) {
        PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) + 1);
    }

    public static string RemoveIntegers(this string input) {
        return Regex.Replace(input, @"[\d-]", string.Empty);
    }

    public static IEnumerator MoveTowards(this GameObject thing, Vector3 startPos, Vector3 finalPos) {
        float elapsedTime = 0f;
        float waitTime = 1f;

        while (elapsedTime < waitTime) {
            thing.transform.position = Vector3.Lerp(startPos, finalPos, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime; 
            yield return null;
        }

        thing.transform.position = finalPos;
        yield return null;
    }

    public static bool IsPointerOverUIElement() {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
    
    public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults) {
        for(int index = 0;  index < eventSystemRaysastResults.Count; index ++) {
            RaycastResult curRaysastResult = eventSystemRaysastResults [index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        return false;
    }

    static List<RaycastResult> GetEventSystemRaycastResults() {   
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position =  Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

}
