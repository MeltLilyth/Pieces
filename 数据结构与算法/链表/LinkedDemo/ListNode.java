package LinkedDemo;

public class ListNode<T> {
    public T value;
    public ListNode<T> nextNode;

    public ListNode(){ }

    public ListNode(T t){
        this.value = t;
    }
}
