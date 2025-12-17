using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ListExtension
{
    /// <summary>
    /// Random lấy 1 id theo trọng số
    /// 
    /// Ví dụ:
    /// var weights = new List<int>() { 10, 30, 60 };
    /// int id = weights.GetIndexByWeight(); 
    /// // id sẽ rơi vào 0, 1, 2 với xác suất 10%, 30%, 60%
    /// </summary>
    public static int GetIndexByWeight(this List<int> lstWeight)
    {
        if (lstWeight == null || lstWeight.Count == 0)
            return -1;

        int total = 0;
        for (int i = 0; i < lstWeight.Count; i++)
            total += Mathf.Max(0, lstWeight[i]); // tránh số âm

        if (total == 0) return -1;

        int randomValue = Random.Range(0, total);
        int cumulative = 0;

        for (int i = 0; i < lstWeight.Count; i++)
        {
            cumulative += Mathf.Max(0, lstWeight[i]);
            if (randomValue < cumulative)
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Random lấy phần tử trong list dựa vào trọng số
    /// 
    /// Ví dụ:
    /// var fruits = new List<string>() { "Apple", "Banana", "Orange" };
    /// var weights = new List<int>() { 10, 30, 60 };
    /// string fruit = fruits.GetByWeight(weights);
    /// // Kết quả trả về theo tỉ lệ trọng số
    /// </summary>
    public static T GetByWeight<T>(this List<T> list, List<int> lstWeight)
    {
        if (list == null || lstWeight == null || list.Count != lstWeight.Count)
            return default;

        int index = lstWeight.GetIndexByWeight();
        if (index < 0) return default;

        return list[index];
    }

    /// <summary>
    /// Trả về danh sách thuộc tính từ danh sách class (có thể thêm điều kiện lọc)
    /// 
    /// Ví dụ:
    /// public class User { public int Id; public string Name; public bool IsActive; }
    ///
    /// var users = new List<User>()
    /// {
    ///     new User { Id = 1, Name = "Alice", IsActive = true },
    ///     new User { Id = 2, Name = "Bob", IsActive = false },
    ///     new User { Id = 3, Name = "Charlie", IsActive = true }
    /// };
    ///
    /// // Lấy toàn bộ Name
    /// var names = users.SelectProperty(u => u.Name);
    ///
    /// // Lấy Name nhưng chỉ user đang active
    /// var activeNames = users.SelectProperty(u => u.Name, u => u.IsActive);
    /// </summary>
    public static List<TResult> SelectProperty<T, TResult>(
        this List<T> list,
        Func<T, TResult> selector,
        Func<T, bool> predicate = null)
    {
        var result = new List<TResult>();
        if (list == null || selector == null) return result;

        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            if (predicate == null || predicate(item))
            {
                result.Add(selector(item));
            }
        }

        return result;
    }

    /// <summary>
    /// Trả về danh sách (phần tử, số lượng), sắp xếp theo số lượng giảm dần
    /// 
    /// Ví dụ:
    /// var fruits = new List<string>() { "Apple", "Banana", "Apple", "Orange", "Banana", "Apple" };
    /// var result = fruits.CountAndSortDescending();
    /// 
    /// // Kết quả:
    /// // Apple - 3
    /// // Banana - 2
    /// // Orange - 1
    /// </summary>
    public static List<(T item, int count)> CountAndSortDescending<T>(this List<T> list)
    {
        var dict = new Dictionary<T, int>();
        foreach (var element in list)
        {
            if (dict.ContainsKey(element))
                dict[element]++;
            else
                dict[element] = 1;
        }

        return dict
            .OrderByDescending(kv => kv.Value)
            .Select(kv => (kv.Key, kv.Value))
            .ToList();
    }

    /// <summary>
    /// Lấy ngẫu nhiên 1 phần tử trong list (có thể thêm điều kiện lọc)
    /// 
    /// Ví dụ:
    /// var numbers = new List<int>() { 1, 2, 3, 4, 5, 6 };
    /// int evenNumber = numbers.GetRandom(n => n % 2 == 0);
    /// // Trả về 2, 4 hoặc 6
    /// </summary>
    public static T GetRandom<T>(this List<T> list, Func<T, bool> predicate = null)
    {
        if (list == null || list.Count == 0)
            return default;

        // Nếu có điều kiện, lọc trước
        var filtered = predicate == null ? list : list.FindAll(new Predicate<T>(predicate));
        if (filtered.Count == 0)
            return default;

        int index = Random.Range(0, filtered.Count);
        return filtered[index];
    }
    /// <summary>
    /// Lấy ngẫu nhiên một số lượng phần tử duy nhất từ danh sách (có điều kiện lọc).
    /// Nếu số lượng yêu cầu lớn hơn số phần tử có thể chọn thì trả về tất cả.
    /// 
    /// Ví dụ:
    /// var numbers = new List<int>() { 1, 2, 2, 3, 4, 4, 5 };
    /// var result = numbers.GetUniqueRandom(3, n => n > 2);
    /// // Kết quả có thể là [3,4,5] hoặc [5,3,4] ... (3 số duy nhất > 2)
    /// </summary>
    public static List<T> GetUniqueRandom<T>(this List<T> list, int count, Func<T, bool> predicate = null)
    {
        if (list == null || list.Count == 0 || count <= 0)
            return new List<T>();

        // Lọc theo điều kiện và lấy phần tử duy nhất
        var filtered = predicate == null ? new HashSet<T>(list) : new HashSet<T>(list.FindAll(new Predicate<T>(predicate)));

        var uniqueList = new List<T>(filtered);
        if (uniqueList.Count == 0)
            return new List<T>();

        // Nếu yêu cầu nhiều hơn số lượng có sẵn -> trả về toàn bộ
        if (count >= uniqueList.Count)
            return uniqueList;

        // Shuffle rồi lấy count phần tử đầu
        for (int i = uniqueList.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (uniqueList[i], uniqueList[j]) = (uniqueList[j], uniqueList[i]);
        }

        return uniqueList.GetRange(0, count);
    }
    /// <summary>
    /// Lấy ngẫu nhiên một số lượng phần tử từ danh sách (không cần duy nhất, có thể trùng nhau).
    /// Nếu danh sách rỗng thì trả về danh sách rỗng.
    /// 
    /// Ví dụ:
    /// var numbers = new List<int>() { 1, 2, 3, 4, 5 };
    /// var result = numbers.GetRandomList(3, n => n % 2 == 0);
    /// // Kết quả có thể là [2,4,2] hoặc [4,2,4] ...
    /// </summary>
    public static List<T> GetRandomList<T>(this List<T> list, int count, Func<T, bool> predicate = null)
    {
        var result = new List<T>();
        if (list == null || list.Count == 0 || count <= 0)
            return result;

        // Lọc theo điều kiện nếu có
        var filtered = predicate == null
            ? new List<T>(list)
            : list.FindAll(new Predicate<T>(predicate));

        if (filtered.Count == 0)
            return result;

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, filtered.Count);
            result.Add(filtered[index]);
        }

        return result;
    }
}
