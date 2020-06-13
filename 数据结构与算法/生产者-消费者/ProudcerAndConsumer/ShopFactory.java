package ProudcerAndConsumer;

import java.util.LinkedList;
import java.util.List;

public class ShopFactory {

    private final int max_num = 10;

    private List<Object> itemList = new LinkedList<Object>();

    public void Produce(){
        synchronized (itemList){
            //如果生产的商品到达临界值，将会暂停该线程运行
            while(this.itemList.size() + 1 > max_num){
                System.out.println("Tread "+ Thread.currentThread().getName()+"将会停止，当前商品数量为： "+itemList.size());
                try{
                    itemList.wait();
                }
                catch(Exception e){
                    e.printStackTrace();
                }
            }
            //如果生产的商品没有到达临界值，将会运行该线程
            this.itemList.add(new Object());
            System.out.println("添加产品操作，当前线程："+Thread.currentThread().getName()+"当前商品数为："+itemList.size());
            this.itemList.notifyAll();
        }
    }

    public void Consume(){
        synchronized (itemList){
            //如果消耗的商品到达临界值，将会暂停该线程运行
            while(itemList.size() == 0){
                System.out.println("Tread "+ Thread.currentThread().getName()+" 将会停止，当前商品数量为： "+itemList.size());
                try{
                    itemList.wait();
                }
                catch(Exception e){
                    e.printStackTrace();
                }
            }
            //如果消耗的商品没有到达临界值，将会运行该线程
            this.itemList.remove(0);
            System.out.println("消耗产品操作，当前线程："+Thread.currentThread().getName()+"当前商品数为："+itemList.size());
            this.itemList.notifyAll();
        }
    }
}
