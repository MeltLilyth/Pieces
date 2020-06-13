package LinkedDemo;

//单向链表
public class LinkedDemo<T> {
    private int size;
    private ListNode<T> rearNode = new ListNode<T>();
    private ListNode<T> frontNode = new ListNode<T>();

    public LinkedDemo(){
        this.size = 0;
        frontNode.nextNode = rearNode;
        rearNode.nextNode = null;
    }

    //添加元素
    public void Add(T t){
        ListNode<T> listNode = new ListNode<T>(t);
        ListNode<T> protoNextNode = frontNode.nextNode;
        frontNode.nextNode = listNode;
        listNode.nextNode = protoNextNode;
        size ++;
    }

    //获取元素
    public T Get(T t){
        int num = 0;
        ListNode<T> resultNode = frontNode.nextNode;
        while(num < size){
            if(resultNode == rearNode || resultNode.value == t){
                break;
            }
            resultNode = resultNode .nextNode;
            num ++;
        }
        return resultNode.value;
    }

    //删除元素
    public void removeNode(T t){
        int num = 0;
        ListNode<T> resultNode = frontNode.nextNode;
        while(num < size){
            if(resultNode.nextNode == rearNode){
                break;
            }
            if(resultNode.nextNode.value == t){
                resultNode.nextNode = resultNode.nextNode.nextNode;
                break;
            }
            resultNode = resultNode.nextNode;
            num ++;
        }
    }
}
