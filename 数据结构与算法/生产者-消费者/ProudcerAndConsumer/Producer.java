package ProudcerAndConsumer;

public class Producer implements Runnable{
    private ShopFactory shopFactory;

    public Producer(ShopFactory shopFactory){
        this.shopFactory = shopFactory;
    }

    @Override
    public void run() {
        while(true){
            try {
                Thread.sleep(1000);
                shopFactory.Produce();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }
}
