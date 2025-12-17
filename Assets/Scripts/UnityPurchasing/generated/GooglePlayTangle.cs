// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Cp2HdAl/MU2SyYgZ2k79HQ2Ag3ebql2op5UfUOzSbTm/gj26QAC7PDaEByQ2CwAPLIBOgPELBwcHAwYFudzyyTortpLgdIwq+IBMvy2J0LEMISJo+aGkr3kBmqszg2RBrDyeWdALIVuIFW0HuK6mqLupAFs07tXJNAGjod2wZhv0Apoq50X3hWuABZgWVcKbMH4sL8ORA9WkU0RxSTn/WYQHCQY2hAcMBIQHBwaxAIjkUysHsYsN8Lk0/h8/z+et5giKdWqACp7ZkG+ZRjfszNKbQ1Jge0A/qep3d9H2QO7/oBkWM1NpdtAdZ0cyCeSCPzZFh/qzv79YOLrlSIriy9OyPgNKzikU5tEJ2az/SIbwLypHIsxE3b0Eekaa6jEw6QQFBwYH");
        private static int[] order = new int[] { 8,10,8,8,8,9,6,11,10,10,13,11,12,13,14 };
        private static int key = 6;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
