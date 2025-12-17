using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EffectTextWare : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;
    public async UniTask StartAnimationText()
    {
        await ApplyImpactEffectToCharacters(tmp);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ApplyImpactEffectToCharacters(tmp);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Test();
        }
    }
    private async UniTask ApplyImpactEffectToCharacters(TextMeshProUGUI tmp)
    {
        float timeScaleUp = 0.2f;
        float timeScaleDown = 0.2f;
        float timeDelay = 0.05f;
        TMP_TextInfo textInfo = tmp.textInfo;
        tmp.ForceMeshUpdate(); // Cập nhật lưới TextMeshPro
        bool anim = true;

        // Ẩn tất cả ký tự ban đầu
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

            // Đặt alpha của tất cả các ký tự về 0
            for (int j = 0; j < 4; j++)
            {
                colors[vertexIndex + j].a = 0;
            }
          
        }
        tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);


        // Lặp qua từng ký tự
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            // Lấy thông tin vị trí của ký tự
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
            Color32[] colors = textInfo.meshInfo[materialIndex].colors32;
            // Lấy trung điểm của ký tự
            Vector3 charMidBaseline = (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) / 2;

            // Lưu tọa độ gốc của các đỉnh ký tự
            Vector3[] originalVertices = new Vector3[4];
            for (int j = 0; j < 4; j++)
            {
                originalVertices[j] = vertices[vertexIndex + j]; // Lưu lại tọa độ ban đầu
            }
            // Hiển thị ký tự bằng cách tăng alpha
            for (int j = 0; j < 4; j++)
            {
                colors[vertexIndex + j].a = 255;
            }
            tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            // Hiệu ứng phóng to từ từ bằng DOTween
            float delay = i * timeDelay; // Thời gian trễ để tạo hiệu ứng từ trái sang phải
            DOTween.To(() => 1f, scale =>
            {
                for (int j = 0; j < 4; j++)
                {
                    vertices[vertexIndex + j] = charMidBaseline + (originalVertices[j] - charMidBaseline) * scale;
                }
                tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            }, 2f, timeScaleUp) // Phóng to (x2)
            .SetDelay(delay) // Áp dụng trễ để hiệu ứng từ trái sang phải
            .SetEase(Ease.OutQuad)

            // Thu nhỏ lại sau khi phóng to
            .OnComplete(() =>
            {
                DOTween.To(() => 2f, scale =>
                {
                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] = charMidBaseline + (originalVertices[j] - charMidBaseline) * scale;
                    }
                    tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                }, 1f, timeScaleDown) // Quay về kích thước ban đầu
                .SetEase(Ease.OutQuad).OnComplete(() => anim = false); ;
            });
        }
        await UniTask.WaitUntil(() => anim == false);
    }
    void Test()
    {
        BendTextToArc(tmp, radius, arcAngle);
    }
    [SerializeField] float radius = 0;
    [SerializeField] float arcAngle = 0;
    private void BendTextToArc(TextMeshProUGUI tmp, float radius, float arcAngle)
    {
        // Cập nhật lưới TextMeshPro
        tmp.ForceMeshUpdate();

        TMP_TextInfo textInfo = tmp.textInfo;
        int characterCount = textInfo.characterCount;

        // Kiểm tra nếu không có ký tự
        if (characterCount == 0) return;

        // Tính toán góc giữa các ký tự
        float angleIncrement = arcAngle / (characterCount - 1);
        float startAngle = -arcAngle / 2; // Bắt đầu từ giữa cung

        Vector3 center = tmp.rectTransform.position; // Tâm cung tròn

        for (int i = 0; i < characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // Lấy trung điểm ban đầu của ký tự
            Vector3 charMidBaseline = (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) / 2;

            // Tính góc và vị trí mới trên cung
            float angle = startAngle + angleIncrement * i;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 targetPosition = new Vector3(
                Mathf.Sin(radians) * radius,
                Mathf.Cos(radians) * radius - radius,
                0f
            );

            // Dịch chuyển ký tự đến vị trí mới
            Vector3 offset = targetPosition - charMidBaseline;
            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] += offset;
            }

            // Xoay ký tự theo góc
            Quaternion rotation = Quaternion.Euler(0, 0, -angle); // Xoay ký tự để căn theo cung
            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] = rotation * (vertices[vertexIndex + j] - targetPosition) + targetPosition;
            }
        }

        // Cập nhật lưới TextMeshPro sau khi thay đổi
        tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }




}