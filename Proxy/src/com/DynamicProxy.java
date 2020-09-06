package com;

import java.lang.reflect.InvocationHandler;
import java.lang.reflect.Method;
import java.lang.reflect.Proxy;
/*
* 动态代理：在运行的时候通过反射的方式动态的创建某些给定类型的类及其实例化对象。
*           因为该方式动态创建的是接口，所以只有在运行的时候才能知道创建的对象的具体实现方式
*
* 动态代理中，InvocationHandler可以通过创建一个类并实现该接口中的Invoke()达到相同的效果
* */

//测试接口
interface ProxyInterface{
    public void TestMethod();
}

//测试方法 -- 被代理类
class ProxyImplement implements ProxyInterface{
    @Override
    public void TestMethod() {
        System.out.println("ProxyImplement Processing");
    }
}

class ProxyFactory{
    //单例 -- 不考虑多线程情况
    private static ProxyFactory _instance;
    public static ProxyFactory getInstance(){
        if(_instance == null){
            _instance = new ProxyFactory();
        }
        return _instance;
    }

    //代理工厂生成代理类并执行指定方法
    public Object InvokeMethod(Object inst){
        return Proxy.newProxyInstance(inst.getClass().getClassLoader(), inst.getClass().getInterfaces(), new InvocationHandler() {
            //可以插入各种前置操作或者后置操作
            @Override
            public Object invoke(Object o, Method method, Object[] objects) throws Throwable {
                Object result = method.invoke(inst,objects);
                return result;
            }
        });
    }
}

public class DynamicProxy {
    public static void main(String[] args){
        ProxyInterface proxyInterface  = (ProxyInterface) ProxyFactory.getInstance().InvokeMethod(new ProxyImplement());
        //实际执行的是被代理类ProxyImplement中的TestMethod方法
        proxyInterface.TestMethod();
    }
}
