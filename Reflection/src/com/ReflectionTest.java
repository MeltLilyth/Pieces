package com;

import java.io.File;
import java.lang.reflect.Constructor;
import java.util.HashMap;

public class ReflectionTest {
    public void createUserClass() throws Exception {
        Class clazz = User.class;
        Constructor cons = clazz.getConstructor();
        User user = (User) cons.newInstance();
        user.setUserId("123");
        user.setUsername("Meltrylils");
        System.out.println(user);
    }

    public static void main(String[] args) throws Exception {
        ReflectionFactory.ScannerPackage(ReflectionFactory.rootPath);

        User user = (User)ReflectionFactory.getInstance().GetBean("user");
        user.setUserId("123");
        user.setUsername("Meltrylils");

        System.out.println(user);
    }
}

//Question: 如果ReflectionFactory中不写空构造函数，clazz.getConstructor()将会找不到该类的构造函数从而报空，后续将会查明
class ReflectionFactory{
    private static HashMap<String,Object> objectMap = new HashMap<String, Object>();
    public static String rootPath = System.getProperty("java.class.path");

    private volatile static ReflectionFactory _instance;

    public static ReflectionFactory getInstance(){
        if(_instance == null){
            synchronized(ReflectionFactory.class){
                if(_instance == null){
                    _instance = new ReflectionFactory();
                }
            }
        }

        return _instance;
    }

    public ReflectionFactory(){ }

    public static void ScannerPackage(String filePath) throws Exception {
        File rootFile = new File(filePath);
        File[] files = rootFile.listFiles();
        for(File childFile : files){
            if(childFile.isDirectory()){
                ScannerPackage(childFile.getAbsolutePath());
                return;
            }
            else if(childFile.getName().endsWith(".class")){
                String classPath = childFile.getAbsolutePath().replaceAll("\\\\",".").split(rootPath.replaceAll("\\\\","."))[1];
                String pkgPath = classPath.substring(1,classPath.length()-6);
                Class clazz = Class.forName(pkgPath);
                Constructor cons = clazz.getConstructor();
                objectMap.put(childFile.getName().replaceAll(".class","").toLowerCase(),cons.newInstance());
            }
        }
    }

    public Object GetBean(String beanName){
        return objectMap.get(beanName);
    }
}

