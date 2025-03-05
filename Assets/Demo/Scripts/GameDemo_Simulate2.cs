using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public partial class GameDemo : MonoBehaviour
{

    void Pad_Simulate()
    {
        bool inverse = true;
        float rota = 70f;

        Vector3 v = new Vector3(1.5f, -1f, 0);

        int inv = 1;
        if (inverse)
            inv = -1;

        v = new Vector3(v.x * inv, v.y, v.z);

        ball.SetVelocity(v);
        physicsStateList[0].velocity = v;

        Pad _pad = InstantPad(ball.transform.position, ball.velocity, currentFrame, rota * inv, true);

        Vector3 center = Vector2.zero;
        Vector3 size = Vector2.zero;
        Vector3 bestPos = Vector2.zero;

        if (ball.velocity.x > 0)
        {
            center = verticalCenter2;
            bestPos = verticalBestPos2;
        }
        else
        {
            center = verticalCenter1;
            bestPos = verticalBestPos1;
        }

        _pad.SetBounds(center, verticalSize, bestPos);

        int simlateFrame = 0;
        for (int i = 0; i < 6; i++)
        {
            simulateNextFrame();
            simlateFrame++;
        }

        PadCurve padCurve = new PadCurve();
        padCurve.rotation = rota * inv;
        padCurve.frame = physicsStateList.Count - 1;
        padCurve.inverse = inverse;
        padCurve.physicsStates = physicsStateList.GetRange(1, physicsStateList.Count - 1);
        padCurve.minFrame = 6;
        padCurve.maxFrame = 9;

        string json_str = JsonUtility.ToJson(padCurve);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "padCurve/temp.json"), json_str);
    }

    void Pad_Simulate2()
    {
        bool inverse = true;
        float rota = 60f;

        Vector3 v = new Vector3(1.5f, -1f, 0);

        int inv = 1;
        if (inverse)
            inv = -1;

        v = new Vector3(v.x * inv, v.y, v.z);

        ball.SetVelocity(v);
        physicsStateList[0].velocity = v;

        Pad _pad = InstantPad(ball.transform.position, ball.velocity, currentFrame, rota * inv, true);

        Vector3 center = Vector2.zero;
        Vector3 size = Vector2.zero;
        Vector3 bestPos = Vector2.zero;

        if (ball.velocity.x > 0)
        {
            center = verticalCenter2;
            bestPos = verticalBestPos2;
        }
        else
        {
            center = verticalCenter1;
            bestPos = verticalBestPos1;
        }

        _pad.SetBounds(center, verticalSize, bestPos);

        int simlateFrame = 0;
        for (int i = 0; i < 10; i++)
        {
            simulateNextFrame();
            simlateFrame++;
        }

        PadCurve padCurve = new PadCurve();
        padCurve.rotation = rota * inv;
        padCurve.frame = physicsStateList.Count - 1;
        padCurve.inverse = inverse;
        padCurve.physicsStates = physicsStateList.GetRange(1, physicsStateList.Count - 1);
        padCurve.minFrame = 10;
        padCurve.maxFrame = 12;

        string json_str = JsonUtility.ToJson(padCurve);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "padCurve/temp.json"), json_str);
    }

    void Bars_Simulate()
    {
        int backFrame = currentFrame;

        foreach (var rotation in h_rotationList1)
        {
            for (int j = 0; j < h_bouncinessPriorityList.Count; j++)
            {
                float bounciness = h_bouncinessPriorityList[j];
                simulateToFrame(backFrame, true, true);
                SetRotation(1, rotation);
                SetPhysicMaterialBounciness(1, bounciness);

                Vector3 prePos = ball.transform.position;

                int simlateFrame = 0;

                for (int i = 0; i < 999; i++)
                {
                    simulateNextFrame();
                    simlateFrame++;

                    Vector3 afterPos = ball.transform.position;

                    if (afterPos.y < prePos.y - 0.1f)
                    {
                        break;
                    }
                }

                BarsDemo barsDemo = InstantBars_Simulate(ball.transform.position, ball.velocity, simulateBarsDemo, false);

                for (int i = 0; i < 999; i++)
                {
                    simulateNextFrame();
                    simlateFrame++;

                    if (barsDemo.isPassXY(ball.transform.position, 0.1f))
                    {
                        break;
                    }
                }
                DestroyImmediate(barsDemo.gameObject);
                Debug.Log($"simlateFrame:{simlateFrame}");
            }
        }
    }


    IEnumerator Bars_Simulate2()
    {
        Physics.gravity = new Vector3(0, -5f, 0);

        int prefabIndex = 1;
        bool inverse = true;
        //Vector3 v = new Vector3(0.829f, -1.29f, 0);
        Vector3 v = new Vector3(0.35f, -0.5f, 0);
        //Vector3 v = new Vector3(-1f, -1f, 0);
        int inv = 1;
        if (inverse)
            inv = -1;

        v = new Vector3(v.x * inv, v.y, 0);
        //Vector3 v = new Vector3(2f, 0f, 0);

        int simlateFrame = 0;
        BarsDemo barsDemo = InstantBars_Simulate(ball.transform.position, ball.velocity, barsDemoPrefabList[prefabIndex], inverse);

        ball.SetVelocity(v);
        physicsStateList[0].velocity = v;

        yield return new WaitForSeconds(0.3f);

        ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x, ball.angularVelocity.y - 5f, ball.angularVelocity.z));
        physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;


        for (int i = 1; i < 20; i++)
        {
            ModifyFrameVelocity(i, 0.15f, 0f);
        }

        for (int i = 40; i < 55; i++)
        {
            ModifyFrameVelocity(i, 0.11f, 0f);
        }

        //for (int i = 30; i < 35; i++)
        //{
        //    ModifyFrameVelocity(i, 0.105f, 0f);
        //}

        //for (int i = 34; i < 40; i++)
        //{
        //    ModifyFrameVelocity(i, 0.1f, 0f);
        //}

        //for (int i = 0; i < 84; i++)
        for (int i = 0; i < 80; i++)
        {
            simulateNextFrame();
            simlateFrame++;

            ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x, ball.angularVelocity.y - 5f, ball.angularVelocity.z));
            physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;
        }

        Debug.Log($"physicsStateList.Count:{physicsStateList.Count}");

        BarsCurve barsCurve = new BarsCurve();
        barsCurve.prefabIndex = prefabIndex;
        barsCurve.inverse = inverse;
        barsCurve.physicsStates = physicsStateList.GetRange(1, physicsStateList.Count - 1);
        barsCurve.frame = physicsStateList.Count - 1;

        string json_str = JsonUtility.ToJson(barsCurve);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "barCurve/temp.json"), json_str);

        logger($"simlateFrame:{simlateFrame}");
        Debug.Log($"simlateFrame:{simlateFrame}");
    }

    IEnumerator Bars_Simulate2_2()
    {
        Physics.gravity = new Vector3(0, -5f, 0);

        int prefabIndex = 1;
        bool inverse = true;
        //Vector3 v = new Vector3(0.829f, -1.29f, 0);
        Vector3 v = new Vector3(-0.35f, -0.5f, 0);
        //Vector3 v = new Vector3(-1f, -1f, 0);
        int inv = 1;
        if (inverse)
            inv = -1;

        v = new Vector3(v.x * inv, v.y, 0);
        //Vector3 v = new Vector3(2f, 0f, 0);

        int simlateFrame = 0;
        BarsDemo barsDemo = InstantBars_Simulate(ball.transform.position, ball.velocity, barsDemoPrefabList[prefabIndex], inverse);

        ball.SetVelocity(v);
        physicsStateList[0].velocity = v;

        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < 9; i++)
        {
            simulateNextFrame();
            simlateFrame++;

            if (barsDemo.isPassX(ball.transform.position))
            {
                break;
            }
        }

        for (int i = 10; i < 40; i++)
        {
            ModifyFrameVelocity(i, 0.2f, 0f);
        }

        for (int i = 9; i < 70; i++)
        {
            simulateNextFrame();
            simlateFrame++;

            if (barsDemo.isPassX(ball.transform.position))
            {
                break;
            }
        }


        Debug.Log($"physicsStateList.Count:{physicsStateList.Count}");

        BarsCurve barsCurve = new BarsCurve();
        barsCurve.prefabIndex = prefabIndex;
        barsCurve.inverse = inverse;
        barsCurve.physicsStates = physicsStateList.GetRange(1, physicsStateList.Count - 1);
        barsCurve.frame = physicsStateList.Count - 1;

        string json_str = JsonUtility.ToJson(barsCurve);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "barCurve/temp.json"), json_str);

        logger($"simlateFrame:{simlateFrame}");
        Debug.Log($"simlateFrame:{simlateFrame}");
    }

    void Bars_Simulate3()
    {
        int prefabIndex = 1;
        bool inverse = false;

        InstantBars_Simulate(ball.transform.position, ball.velocity, barsDemoPrefabList[prefabIndex], inverse);
        BarsCurve barsCurve = FindBarsCurve(prefabIndex, inverse);
        //List<PhysicsState> physicsStates = TransformPhysicsState_Simulate(barsCurve.physicsStates);
        List<PhysicsState> physicsStates = TransformPhysicsState_Simulate(barsCurve.physicsStates);
        AddPhysicsState(physicsStates);
        simulateToFrame(physicsStates.Count, true, true);

        barsCurve.physicsStates = physicsStateList.GetRange(1, physicsStateList.Count - 1);
        barsCurve.frame = physicsStateList.Count - 1;

        string json_str = JsonUtility.ToJson(barsCurve);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "barCurve/temp.json"), json_str);
    }

    IEnumerator Bars_Simulate4()
    {
        Physics.gravity = new Vector3(0, -5f, 0);

        int prefabIndex = 2;
        bool inverse = true;
        Vector3 v = new Vector3(1f, -0.9f, 0);
        int inv = 1;
        if (inverse)
            inv = -1;

        v = new Vector3(v.x * inv, v.y, 0);

        int simlateFrame = 0;
        BarsDemo barsDemo = InstantBars_Simulate(ball.transform.position, ball.velocity, simulateBarsDemo, inverse);

        ball.SetVelocity(v);
        physicsStateList[0].velocity = v;

        yield return new WaitForSeconds(0.3f);

        ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x, ball.angularVelocity.y - 1f, ball.angularVelocity.z));
        physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;

        for (int i = 2; i < 5; i++)
        {
            //ModifyFrameVelocity(i, 5f, 5f);
            //ModifyFrameVelocity(i, 2f, 2f);
        }

        for (int i = 5; i < 12; i++)
        {
            //ModifyFrameVelocity(i, -0.2f, 0);
        }

        for (int i = 0; i < 12; i++)
        {
            simulateNextFrame(true, false);
            simlateFrame++;

            ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x, ball.angularVelocity.y - 1f, ball.angularVelocity.z));
            physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;
        }

        BarsCurve barsCurve = new BarsCurve();
        barsCurve.prefabIndex = prefabIndex;
        barsCurve.inverse = inverse;
        barsCurve.physicsStates = physicsStateList.GetRange(1, physicsStateList.Count - 1);
        barsCurve.frame = physicsStateList.Count - 1;

        string json_str = JsonUtility.ToJson(barsCurve);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "barCurve/temp.json"), json_str);

        logger($"simlateFrame:{simlateFrame}");
        Debug.Log($"simlateFrame:{simlateFrame}");
    }


    IEnumerator Bars_Simulate5()
    {
        Physics.gravity = new Vector3(0, -5f, 0);

        int prefabIndex = 3;
        bool inverse = true ;
        Vector3 v = new Vector3(1f, -0.9f, 0);
        int inv = 1;
        if (inverse)
            inv = -1;

        v = new Vector3(v.x * inv, v.y, 0);

        int simlateFrame = 0;
        BarsDemo barsDemo = InstantBars_Simulate(ball.transform.position, ball.velocity, simulateBarsDemo, inverse);

        ball.SetVelocity(v);
        physicsStateList[0].velocity = v;

        yield return new WaitForSeconds(0.3f);

        ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x, ball.angularVelocity.y - 1f, ball.angularVelocity.z));
        physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;

        for (int i = 2; i < 5; i++)
        {
            //ModifyFrameVelocity(i, 5f, 5f);
            //ModifyFrameVelocity(i, 2f, 2f);
        }

        for (int i = 5; i < 15; i++)
        {
            //ModifyFrameVelocity(i, -0.2f, 0);
        }

        for (int i = 0; i < 15; i++)
        {
            simulateNextFrame(true, false);
            simlateFrame++;

            ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x, ball.angularVelocity.y - 1f, ball.angularVelocity.z));
            physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;
        }

        BarsCurve barsCurve = new BarsCurve();
        barsCurve.prefabIndex = prefabIndex;
        barsCurve.inverse = inverse;
        barsCurve.physicsStates = physicsStateList.GetRange(1, physicsStateList.Count - 1);
        barsCurve.frame = physicsStateList.Count - 1;

        string json_str = JsonUtility.ToJson(barsCurve);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "barCurve/temp.json"), json_str);

        logger($"simlateFrame:{simlateFrame}");
        Debug.Log($"simlateFrame:{simlateFrame}");
    }

    IEnumerator Bars_Simulate6()
    {
        Physics.gravity = new Vector3(0, -5f, 0);

        int prefabIndex = 4;
        bool inverse = true;
        Vector3 v = new Vector3(0.6f, -0.9f, 0);
        int inv = 1;
        if (inverse)
            inv = -1;

        v = new Vector3(v.x * inv, v.y, 0);

        int simlateFrame = 0;
        BarsDemo barsDemo = InstantBars_Simulate(ball.transform.position, ball.velocity, simulateBarsDemo, inverse);

        ball.SetVelocity(v);
        physicsStateList[0].velocity = v;

        yield return new WaitForSeconds(0.3f);

        ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x, ball.angularVelocity.y - 1f, ball.angularVelocity.z));
        physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;

        for (int i = 5; i < 18; i++)
        {
            ModifyFrameVelocity(i, -0.2f, 0);
        }

        for (int i = 0; i < 18; i++)
        {
            simulateNextFrame(true, false);
            simlateFrame++;

            ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x, ball.angularVelocity.y - 1f, ball.angularVelocity.z));
            physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;
        }

        BarsCurve barsCurve = new BarsCurve();
        barsCurve.prefabIndex = prefabIndex;
        barsCurve.inverse = inverse;
        barsCurve.physicsStates = physicsStateList.GetRange(1, physicsStateList.Count - 1);
        barsCurve.frame = physicsStateList.Count - 1;

        string json_str = JsonUtility.ToJson(barsCurve);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "barCurve/temp.json"), json_str);

        logger($"simlateFrame:{simlateFrame}");
        Debug.Log($"simlateFrame:{simlateFrame}");
    }

    IEnumerator Bars_Simulate7()
    {
        Physics.gravity = new Vector3(0, -5f, 0);

        int prefabIndex = 5;
        bool inverse = true;
        Vector3 v = new Vector3(0.6f, -1.7f, 0);
        //Vector3 v = new Vector3(0.6f, -2f, 0);
        int inv = 1;
        if (inverse)
            inv = -1;

        v = new Vector3(v.x * inv, v.y, 0);

        int simlateFrame = 0;
        BarsDemo barsDemo = InstantBars_Simulate(ball.transform.position, ball.velocity, simulateBarsDemo, inverse);

        ball.SetVelocity(v);
        physicsStateList[0].velocity = v;

        yield return new WaitForSeconds(0.3f);

        ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x, ball.angularVelocity.y - 1f, ball.angularVelocity.z));
        physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;

        //for (int i = 5; i < 21; i++)
        //{
        //    ModifyFrameVelocity(i, -0.2f, 0);
        //}

        for (int i = 0; i < 21; i++)
        {
            simulateNextFrame(true, false);
            simlateFrame++;

            ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x, ball.angularVelocity.y - 1f, ball.angularVelocity.z));
            physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;
        }

        BarsCurve barsCurve = new BarsCurve();
        barsCurve.prefabIndex = prefabIndex;
        barsCurve.inverse = inverse;
        barsCurve.physicsStates = physicsStateList.GetRange(1, physicsStateList.Count - 1);
        barsCurve.frame = physicsStateList.Count - 1;

        string json_str = JsonUtility.ToJson(barsCurve);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "barCurve/temp.json"), json_str);

        logger($"simlateFrame:{simlateFrame}");
        Debug.Log($"simlateFrame:{simlateFrame}");
    }

    IEnumerator Bars_Simulate8()
    {
        Physics.gravity = new Vector3(0, -5f, 0);

        int prefabIndex = 6;
        bool inverse = false;
        Vector3 v = new Vector3(0.6f, -2f, 0);
        //Vector3 v = new Vector3(0.6f, -2f, 0);
        int inv = 1;
        if (inverse)
            inv = -1;

        v = new Vector3(v.x * inv, v.y, 0);

        int simlateFrame = 0;
        BarsDemo barsDemo = InstantBars_Simulate(ball.transform.position, ball.velocity, simulateBarsDemo, inverse);

        ball.SetVelocity(v);
        physicsStateList[0].velocity = v;

        yield return new WaitForSeconds(0.3f);

        ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x, ball.angularVelocity.y - 1f, ball.angularVelocity.z));
        physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;

        //for (int i = 5; i < 21; i++)
        //{
        //    ModifyFrameVelocity(i, -0.2f, 0);
        //}

        for (int i = 0; i < 23; i++)
        {
            simulateNextFrame(true, false);
            simlateFrame++;

            ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x, ball.angularVelocity.y - 1f, ball.angularVelocity.z));
            physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;
        }

        BarsCurve barsCurve = new BarsCurve();
        barsCurve.prefabIndex = prefabIndex;
        barsCurve.inverse = inverse;
        barsCurve.physicsStates = physicsStateList.GetRange(1, physicsStateList.Count - 1);
        barsCurve.frame = physicsStateList.Count - 1;

        string json_str = JsonUtility.ToJson(barsCurve);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "barCurve/temp.json"), json_str);

        logger($"simlateFrame:{simlateFrame}");
        Debug.Log($"simlateFrame:{simlateFrame}");
    }

    IEnumerator Bars_Simulate9()
    {
        Physics.gravity = new Vector3(0, -5f, 0);

        int prefabIndex = 7;
        bool inverse = true;
        Vector3 v = new Vector3(0.6f, -2f, 0);
        //Vector3 v = new Vector3(0.6f, -2f, 0);
        int inv = 1;
        if (inverse)
            inv = -1;

        v = new Vector3(v.x * inv, v.y, 0);

        int simlateFrame = 0;
        BarsDemo barsDemo = InstantBars_Simulate(ball.transform.position, ball.velocity, simulateBarsDemo, inverse);

        ball.SetVelocity(v);
        physicsStateList[0].velocity = v;

        for (int i = 10; i < 20; i++)
        {
            ModifyFrameVelocity(i, 0.2f, 0);
        }

        yield return new WaitForSeconds(0.3f);

        ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x, ball.angularVelocity.y - 1f, ball.angularVelocity.z));
        physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;

        //for (int i = 5; i < 21; i++)
        //{
        //    ModifyFrameVelocity(i, -0.2f, 0);
        //}

        for (int i = 0; i < 27; i++)
        {
            simulateNextFrame(true, false);
            simlateFrame++;

            ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x, ball.angularVelocity.y - 1f, ball.angularVelocity.z));
            physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;
        }

        BarsCurve barsCurve = new BarsCurve();
        barsCurve.prefabIndex = prefabIndex;
        barsCurve.inverse = inverse;
        barsCurve.physicsStates = physicsStateList.GetRange(1, physicsStateList.Count - 1);
        barsCurve.frame = physicsStateList.Count - 1;

        string json_str = JsonUtility.ToJson(barsCurve);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "barCurve/temp.json"), json_str);

        logger($"simlateFrame:{simlateFrame}");
        Debug.Log($"simlateFrame:{simlateFrame}");
    }

    IEnumerator Bars_Simulate10()
    {
        Physics.gravity = new Vector3(0, -5f, 0);

        int prefabIndex = 8;
        bool inverse = false;
        Vector3 v = new Vector3(0.6f, -2f, 0);
        //Vector3 v = new Vector3(0.6f, -2f, 0);
        int inv = 1;
        if (inverse)
            inv = -1;

        v = new Vector3(v.x * inv, v.y, 0);

        int simlateFrame = 0;
        BarsDemo barsDemo = InstantBars_Simulate(ball.transform.position, ball.velocity, simulateBarsDemo, inverse);

        ball.SetVelocity(v);
        physicsStateList[0].velocity = v;
        for (int i = 10; i < 11; i++)
        {
            ModifyFrameVelocity(i, 0.3f, 0);
        }

        yield return new WaitForSeconds(0.3f);

        ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x, ball.angularVelocity.y - 1f, ball.angularVelocity.z));
        physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;

        //for (int i = 5; i < 21; i++)
        //{
        //    ModifyFrameVelocity(i, -0.2f, 0);
        //}

        for (int i = 0; i < 30; i++)
        {
            simulateNextFrame(true, false);
            simlateFrame++;

            ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x, ball.angularVelocity.y - 1f, ball.angularVelocity.z));
            physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;
        }

        BarsCurve barsCurve = new BarsCurve();
        barsCurve.prefabIndex = prefabIndex;
        barsCurve.inverse = inverse;
        barsCurve.physicsStates = physicsStateList.GetRange(1, physicsStateList.Count - 1);
        barsCurve.frame = physicsStateList.Count - 1;

        string json_str = JsonUtility.ToJson(barsCurve);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "barCurve/temp.json"), json_str);

        logger($"simlateFrame:{simlateFrame}");
        Debug.Log($"simlateFrame:{simlateFrame}");
    }
}
