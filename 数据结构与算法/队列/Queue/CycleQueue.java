package Queue;

public class CycleQueue<T> {
    private final int maxNum = 10;
    private int rearNode;
    private int frontNode;
    private T[] tArray;

    public CycleQueue(){
        this.rearNode = 0;
        this.frontNode = 0;
    }

    //入队
    public void Add(T t){
        if((rearNode + 1) % maxNum == frontNode){
            System.out.println("The CycleQueue is full of items");
            return;
        }
        tArray[rearNode] = t;
        rearNode = (rearNode+1)% maxNum;
    }

    //出队
    public void removeNode(){
        if(rearNode == frontNode){
            System.out.println("The CycleQueue is empty");
            return;
        }
        tArray[frontNode] = null;
        frontNode = (frontNode + 1)% maxNum;
    }

}
