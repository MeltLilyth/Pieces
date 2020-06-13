package ProudcerAndConsumer;

public class Consumer implements Runnable {
    private ShopFactory shopFactory;

    public Consumer(ShopFactory shopFactory){
        this.shopFactory = shopFactory;
    }

    @Override
    public void run() {
        while(true){
            try{
                Thread.sleep(3000);
                shopFactory.Consume();
            }
            catch(Exception e){
                e.printStackTrace();
            }
        }
    }
}
