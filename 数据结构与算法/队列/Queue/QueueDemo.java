package Queue;

import java.util.Arrays;

//队列 -- 遵循先进先出的原则
public class QueueDemo<T> {
    private final int maxNum = 10;
    private T[] queueArray;
    private int rearNode;

    public QueueDemo(){
        this.rearNode = 0;
    }

    public void Add(T t){
        if(rearNode < maxNum){
            queueArray[rearNode] = t;
            rearNode ++;
            return;
        }
        System.out.println("The Queue is full.");
    }

    private void MoveArray(){
        queueArray[0] = null;
        this.queueArray = Arrays.copyOfRange(this.queueArray,1,rearNode);
    }

    public T Get(){
        if(rearNode != 0){
            T result = queueArray[0];
            MoveArray();
            rearNode --;
            return result;
        }
        System.out.println("The Queue is empty.");
        return null;
    }
}
