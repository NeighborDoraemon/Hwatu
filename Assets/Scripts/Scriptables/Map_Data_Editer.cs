using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Map_Data_Editer : MonoBehaviour
{
    [MenuItem("Tools/Apply Enemy Spawn Data to Selected Map")]
    static void ApplyEnemySpawnData()
    {
        // 선택된 ScriptableObject 가져오기
        Map_Value spawnData = Selection.activeObject as Map_Value;
        if (spawnData == null)
        {
            Debug.LogError("선택된 Map_Value ScriptableObject가 없습니다! Project 창에서 원하는 Map_Value를 선택하세요.");
            return;
        }

        List<Vectors> waveSpawnPoints = new List<Vectors>();
        List<e_Numbers> waveEnemyIndexes = new List<e_Numbers>();

        // "Wave" 태그가 있는 모든 오브젝트 가져오기
        GameObject[] waveObjects = GameObject.FindGameObjectsWithTag("Wave");

        foreach (var wave in waveObjects.OrderBy(w => w.name))
        {
            Vectors waveVectors = new Vectors();
            e_Numbers waveIndexes = new e_Numbers();

            foreach (Transform child in wave.transform)
            {
                waveVectors.v_Dataes.Add(child.position);

                EnemySpawnPoint enemyData = child.GetComponent<EnemySpawnPoint>();
                if (enemyData != null)
                {
                    waveIndexes.i_enemy_Index.Add(enemyData.enemyIndex);
                }
            }

            waveSpawnPoints.Add(waveVectors);
            waveEnemyIndexes.Add(waveIndexes);
        }

        // 기존 데이터 유지하면서 새로운 데이터만 추가
        spawnData.v_New_Spawn_Points = waveSpawnPoints;
        spawnData.i_Enemy_Index = waveEnemyIndexes;
        spawnData.i_How_Many_Wave = waveSpawnPoints.Count;

        // 변경 사항 저장
        EditorUtility.SetDirty(spawnData);
        AssetDatabase.SaveAssets();

        Debug.Log($"[{spawnData.name}]에 적 배치 데이터 추가 완료!");

        // 변환 후 씬 내 빈 오브젝트 삭제
        foreach (var wave in waveObjects)
        {
            DestroyImmediate(wave);
        }
    }
}
