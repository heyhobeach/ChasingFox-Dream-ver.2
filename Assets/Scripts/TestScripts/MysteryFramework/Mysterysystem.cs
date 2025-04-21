using UnityEngine;

public class Mysterysystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is create
    /// <summary>
    /// 흔적으로 받을 변수들
    /// </summary>
    public string[] word1 = { "설리번","발톱","칼날","아티스트" };
    public string[] word2 = { "로벨리아","나","조지" };
    public string[] word3 = { "사랑","살해","의심" };

    public int word1Index,word2Index,word3Index;

    public string word1_mark, word2_mark, word3_mark;

    public string resutl;

    public string answer = "설리번은나를의심했다";

    private void Update()
    {
        resutl = string.Format("{0}{1}{2}{3}{4}{5}", word1[word1Index], word1_mark, word2[word2Index], word2_mark, word3[word3Index],word3_mark);

        if (resutl == answer)
        {
            Debug.Log("정답");
        }
    }
    //string result = string.Format("{0}가 {1}를 {2}했다",word1,word2,word3);
}
