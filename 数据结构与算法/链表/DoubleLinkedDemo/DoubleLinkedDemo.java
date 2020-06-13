package DoubleLinkedDemo;

//双向链表
public class DoubleLinkedDemo<T> {
    private int size;
    private ListNode<T> frontNode = new ListNode<T>();
    private ListNode<T> rearNode = new ListNode<T>();

    public DoubleLinkedDemo(){
        this.size = 0;
        this.frontNode.nextNode = rearNode;
        this.rearNode.preNode = frontNode;
    }

    //添加元素
    public void Add(T t){
        ListNode<T> listNode = new ListNode<T>(t);
        rearNode.preNode.nextNode = listNode;
        listNode.preNode = rearNode.preNode;
        listNode.nextNode = rearNode;
        rearNode.preNode = listNode;

        size ++;
    }

    //返回指定元素
    public T Get(T t){
        int num = 0;
        ListNode<T> resultNode = frontNode.nextNode;
        while(num < size){
            if(resultNode.value == t){
                break;
            }
            if(num == size){
                resultNode = rearNode;
                break;
            }
            resultNode = resultNode.nextNode;
            num ++;
        }

        return resultNode.value;
    }

    //删除指定元素
    public void removeNode(T t){
        int num = 0;
        ListNode<T> resultNode = frontNode.nextNode;
        while(resultNode != rearNode){
            if(resultNode.value == t){
                resultNode.preNode.nextNode = resultNode.nextNode;
                resultNode.nextNode.preNode = resultNode.preNode;
                resultNode = null;
                break;
            }
            resultNode = resultNode.nextNode;
            num ++ ;
        }
    }
}
