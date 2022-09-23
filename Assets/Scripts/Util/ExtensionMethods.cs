using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;

public static class ExtensionMethods
{
    ///
    /// this script is a collection of extension methods
    /// 

    #region // list extensions

    // shuffles every element of the specified list
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    /// return a random item from the list
    /// sampling with replacement
    public static T RandomItem<T>(this IList<T> list)
    {
        if (list.Count == 0) throw new System.IndexOutOfRangeException("Cannot select a random item from an empty list");
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    // removes a random item from a list
    public static T RemoveRandom<T>(this IList<T> list)
    {
        if (list.Count == 0) throw new System.IndexOutOfRangeException("Cannot remove a random item from an empty list");
        int index = UnityEngine.Random.Range(0, list.Count);
        T item = list[index];
        list.RemoveAt(index);
        return item;
    }

    #endregion

    #region // vector extensions

    // returns a random Vector3 within the range, radius
    public static Vector3 AddRandomRadius(this Vector3 vector, float radius)
    {
        // randomize the variables
        float x = UnityEngine.Random.Range(-radius, radius);
        float y = UnityEngine.Random.Range(-radius, radius);
        float z = UnityEngine.Random.Range(-radius, radius);
        // set the vector
        return new Vector3(x, y, z);
    }

    // overloading method
    public static Vector3 AddRandomRadius(this Vector3 vector, float xRadius, float yRadius, float zRadius)
    {
        // randomize variables
        float x = UnityEngine.Random.Range(-xRadius, xRadius);
        float y = UnityEngine.Random.Range(-yRadius, yRadius);
        float z = UnityEngine.Random.Range(-zRadius, zRadius);
        // set the vector
        return new Vector3(x, y, z);
    }

     // returns a random Vector2 within the range, radius
    public static Vector2 AddRandomRadius(this Vector2 vector, float radius)
    {
        // randomize the variables
        float x = UnityEngine.Random.Range(-radius, radius);
        float y = UnityEngine.Random.Range(-radius, radius);
        // set the vector
        return new Vector2(x, y);
    }

    // overloading method
    public static Vector2 AddRandomRadius(this Vector3 vector, float xRadius, float yRadius)
    {
        // randomize variables
        float x = UnityEngine.Random.Range(-xRadius, xRadius);
        float y = UnityEngine.Random.Range(-yRadius, yRadius);
        // set the vector
        return new Vector2(x, y);
    }

    #endregion

    // unparenting transform
    public static void Unparent(this Transform transform)
    {
        transform.parent = null;
    }

    // ToString
    public static string ToString(this object anObject, string aFormat)
    {
        return ToString(anObject, aFormat, null);
    }

    public static string ToString(this object anObject, string aFormat, IFormatProvider formatProvider)
    {
        StringBuilder sb = new StringBuilder();
        Type type = anObject.GetType();
        Regex reg = new Regex(@"({)([^}]+)(})", RegexOptions.IgnoreCase);
        MatchCollection mc = reg.Matches(aFormat);
        int startIndex = 0;
        foreach (Match m in mc)
        {
            Group g = m.Groups[2]; //it's second in the match between { and }
            int length = g.Index - startIndex - 1;
            sb.Append(aFormat.Substring(startIndex, length));

            string toGet = string.Empty;
            string toFormat = string.Empty;
            int formatIndex = g.Value.IndexOf(":"); //formatting would be to the right of a :
            if (formatIndex == -1) //no formatting, no worries
            {
                toGet = g.Value;
            }
            else //pickup the formatting
            {
                toGet = g.Value.Substring(0, formatIndex);
                toFormat = g.Value.Substring(formatIndex + 1);
            }

            //first try properties
            PropertyInfo retrievedProperty = type.GetProperty(toGet);
            Type retrievedType = null;
            object retrievedObject = null;
            if (retrievedProperty != null)
            {
                retrievedType = retrievedProperty.PropertyType;
                retrievedObject = retrievedProperty.GetValue(anObject, null);
            }
            else //try fields
            {
                FieldInfo retrievedField = type.GetField(toGet);
                if (retrievedField != null)
                {
                    retrievedType = retrievedField.FieldType;
                    retrievedObject = retrievedField.GetValue(anObject);
                }
            }

            if (retrievedType != null) //Cool, we found something
            {
                string result = string.Empty;
                if (toFormat == string.Empty) //no format info
                {
                    result = retrievedType.InvokeMember("ToString",
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase
                        , null, retrievedObject, null) as string;
                }
                else //format info
                {
                    result = retrievedType.InvokeMember("ToString",
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase
                        , null, retrievedObject, new object[] { toFormat, formatProvider }) as string;
                }
                sb.Append(result);
            }
            else //didn't find a property with that name, so be gracious and put it back
            {
                sb.Append("{");
                sb.Append(g.Value);
                sb.Append("}");
            }
            startIndex = g.Index + g.Length + 1;
        }
        if (startIndex < aFormat.Length) //include the rest (end) of the string
        {
            sb.Append(aFormat.Substring(startIndex));
        }
        return sb.ToString();
    }

    // rigibody stuff
    public static void FreezeAll(this Rigidbody rigidbody)
    {
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    // testing
    static void Testing()
    {
        Vector3 test = Vector3.zero;
        test = test.AddRandomRadius(5f);
        
    }

}
