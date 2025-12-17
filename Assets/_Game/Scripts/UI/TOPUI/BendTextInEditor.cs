using TMPro;
using UnityEngine;

public class BendTextInEditor : MonoBehaviour
{
    [SerializeField, Range(0f, 1000f)] private float radius = 300f; // Bán kính điều chỉnh trực quan
    [SerializeField, Range(0f, 180f)] private float arcAnglePerLine = 45f; // Góc cung mỗi hàng
    [SerializeField] private TextMeshProUGUI tmp;

    private void OnValidate()
    {
        if (tmp != null)
        {
          //  BendTextToArcMultiline();
        }
    }

    private void BendTextToArcMultiline()
    {
        // Cập nhật thông tin text mesh
        tmp.ForceMeshUpdate();

        TMP_TextInfo textInfo = tmp.textInfo;

        // Lặp qua từng dòng
        for (int lineIndex = 0; lineIndex < textInfo.lineCount; lineIndex++)
        {
            // Lấy thông tin về dòng
            TMP_LineInfo lineInfo = textInfo.lineInfo[lineIndex];
            int firstCharIndex = lineInfo.firstCharacterIndex;
            int lastCharIndex = lineInfo.lastCharacterIndex;

            // Tính số ký tự trong dòng, bỏ qua nếu chỉ có một ký tự
            int characterCount = lastCharIndex - firstCharIndex + 1;
            if (characterCount <= 1) continue;

            // Tính toán tổng chiều rộng của các ký tự trong dòng
            float totalWidth = 0f;
            for (int i = firstCharIndex; i <= lastCharIndex; i++)
            {
               // totalWidth += textInfo.characterInfo[i].characterWidth;
            }

            // Tính toán góc phân bố đều giữa các ký tự
            float angleIncrement = arcAnglePerLine / (characterCount - 1);
            float startAngle = -arcAnglePerLine / 2;

            // Lặp qua các ký tự trong dòng
            for (int i = firstCharIndex; i <= lastCharIndex; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;

                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                // Lấy thông tin chiều rộng của ký tự
              //  float charWidth = textInfo.characterInfo[i].characterWidth;

                // Tính góc và vị trí mới cho ký tự
                float angle = startAngle + angleIncrement * (i - firstCharIndex);
                float radians = angle * Mathf.Deg2Rad;

                // Tính toán vị trí ký tự trên cung tròn với chiều rộng của ký tự
                Vector3 targetPosition = new Vector3(
                    Mathf.Sin(radians) * radius,
                    Mathf.Cos(radians) * radius - radius - lineIndex * 50f, // Điều chỉnh vị trí theo dòng
                    0f
                );

                // Dịch chuyển ký tự đến vị trí mới
                Vector3 offset = targetPosition - (vertices[vertexIndex + 0] + vertices[vertexIndex + 2]) / 2;
                for (int j = 0; j < 4; j++)
                {
                    vertices[vertexIndex + j] += offset;
                }

                // Xoay ký tự theo góc
                Quaternion rotation = Quaternion.Euler(0, 0, -angle); // Xoay ký tự theo hướng cung
                for (int j = 0; j < 4; j++)
                {
                    vertices[vertexIndex + j] = rotation * (vertices[vertexIndex + j] - targetPosition) + targetPosition;
                }
            }
        }

        // Cập nhật lại mesh sau khi thay đổi
        tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }
}
