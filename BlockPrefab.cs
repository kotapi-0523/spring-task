using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BlockPrefab : MonoBehaviour
{
    // Inspector上で次のシーン名を設定
    public string nextSceneName;

    private float Count;
    public float Timer;
    public GameObject blockprefab;
    public Sprite[] blocksprites;

    private GameObject firstBlock;
    private GameObject lastBlock;
    private string currentName;

    List<GameObject> removableBlockList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DropBlock(50));
    }

    void Update()
    {
        //画面をクリックし，firstblockがnullの実行
        if (Input.GetMouseButtonDown(0) && firstBlock == null)
        {
            OnDragStart();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            //クリックを終えた時
            OnDragEnd();
        }
        else if (firstBlock != null)
        {
            OnDragging();
        }

        Count = Timer - Time.time;
        if(Count <= 0.0f)
        {
            changeNext();
        }
    }

    private void OnDragStart()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            GameObject hitObj = hit.collider.gameObject;
            //オブジェクトの名前を前方一致で判定
            string blockName = hitObj.gameObject.name;

            if (blockName.StartsWith("BLOCK"))
            {
                firstBlock = hitObj;
                lastBlock = hitObj;
                currentName = hitObj.name;
                //削除対象オブジェクトリストの初期化
                removableBlockList = new List<GameObject>();
                //削除対象のオブジェクトを格納
                PushToList(hitObj);
            }
        }
    }

    private void OnDragging()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            GameObject hitObj = hit.collider.gameObject;
            //同じ名前のブロックをクリック＆lastblockとは別オブジェクトである時
            if (hitObj.name == currentName && lastBlock != hitObj)
            {
                //2つのオブジェクトの距離を取得
                float distance = Vector2.Distance(hitObj.transform.position, lastBlock.transform.position);

                if (distance < 1.5f)
                {
                    //削除対象のオブジェクトを格納
                    lastBlock = hitObj;
                    PushToList(hitObj);
                }
            }
        }
    }

    private void OnDragEnd()
    {
        int remove_cnt = removableBlockList.Count;

        if (remove_cnt >= 3)
        {
            for (int i = 0; i < remove_cnt; i++)
            {
                Destroy(removableBlockList[i]);
            }

            StartCoroutine(DropBlock(remove_cnt));
        }
        else
        {
            for (int i = 0; i < remove_cnt; i++)
            {
                ChangeColor(removableBlockList[i], 1.0f);
            }
        }

        firstBlock = null;
        lastBlock = null;
    }

    IEnumerator DropBlock(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 pos = new Vector2(Random.Range(-2.0f, 2.0f), 7f);
            GameObject block = Instantiate(blockprefab, pos, Quaternion.AngleAxis(Random.Range(0, 0), Vector3.forward)) as GameObject;
            int spriteId = Random.Range(0, 4);
            block.name = "BLOCK" + spriteId;
            SpriteRenderer spriteobj = block.GetComponent<SpriteRenderer>();
            spriteobj.sprite = blocksprites[spriteId];
            yield return new WaitForSeconds(0.05f);
        }
    }

    void PushToList(GameObject obj)
    {
        removableBlockList.Add(obj);

        ChangeColor(obj, 0.5f);
    }

    void ChangeColor(GameObject obj, float transparency)
    {
        //SpriteRendererコンポーネントを取得
        SpriteRenderer blockTexture = obj.GetComponent<SpriteRenderer>();
        //Colorプロパティのうち，透明度のみ変更する
        blockTexture.color = new Color(blockTexture.color.r, blockTexture.color.g, blockTexture.color.b, transparency);
    }

    void changeNext()
    {
        if (Time.timeSinceLevelLoad > 10.0f)
        {
            SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }
    }
}
