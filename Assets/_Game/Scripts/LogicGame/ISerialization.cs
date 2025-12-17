using UnityEngine;

public interface ISerialization
{
    void Serialize();
    void InitializeFromSave();
    void ClearData();
}