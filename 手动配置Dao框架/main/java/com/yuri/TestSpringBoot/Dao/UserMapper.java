package com.yuri.TestSpringBoot.Dao;

import com.yuri.TestSpringBoot.Entity.User;
import org.apache.ibatis.annotations.Param;
import java.util.List;

public interface UserMapper {

    public User findUseById(@Param("userId") String userId);

    public List<User> findAllUsers();

    public void addNewUser(@Param("user") User user);

    public void updateUserMessage(@Param("user") User user);

    public void deleteUser(@Param("userId")String userId);
}
