package com.yuri.TestSpringBoot.DataSource;

import com.mchange.v2.c3p0.ComboPooledDataSource;
import org.mybatis.spring.SqlSessionFactoryBean;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.io.support.PathMatchingResourcePatternResolver;

import javax.sql.DataSource;
import java.beans.PropertyVetoException;
import java.io.IOException;

@Configuration
public class DataBaseConfig {
    @Autowired
    private DataSource dataSource;  //自动获取对应的bean

    //配置数据库相关 -- 使用c3p0
    @Bean(name = "dataSource")
    public ComboPooledDataSource createDataBaseConfig() throws PropertyVetoException {
        ComboPooledDataSource comboPooledDataSource = new ComboPooledDataSource();
        //连接驱动
        comboPooledDataSource.setDriverClass("com.mysql.jdbc.driver");
        //数据库地址-限于本地数据库 = 数据库本机地址:端口号/数据库名称
        //serverTimezone: 设定时区 -- 存储的时候会转化成UTC时间戳，读取时再从UTC转化为本地时间戳
        //characterEncoding: 编码格式 utf-8支持中文编码(否则数据库中的中文数据在处理过程中会乱码)
        comboPooledDataSource.setJdbcUrl("jdbc:mysql://localhost:3306/springboot?serverTimezone=UTC&characterEncoding=utf-8");
        //用户名
        comboPooledDataSource.setUser("root");
        //密码
        comboPooledDataSource.setPassword("wrj1029");
        //设置数据库关闭之后不进行数据更新
        comboPooledDataSource.setAutoCommitOnClose(false);

        return comboPooledDataSource;
    }

    @Bean(name = "sqlSessionFactory")
    public SqlSessionFactoryBean createSqlSessionBean() throws IOException {
        SqlSessionFactoryBean sqlSessionFactoryBean = new SqlSessionFactoryBean();

        //加载mybatis-config.xml配置文件
        sqlSessionFactoryBean.setConfigLocation(new PathMatchingResourcePatternResolver().getResource("mybatis-config.xml"));
        //设置Mapper扫描包
        sqlSessionFactoryBean.setMapperLocations(new PathMatchingResourcePatternResolver().getResources("Mapper/*.xml"));
        //设置dataSource配置
        sqlSessionFactoryBean.setDataSource(dataSource);

        return sqlSessionFactoryBean;
    }
}
