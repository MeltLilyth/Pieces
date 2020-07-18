package com.yuri.TestSpringBoot.Entity;

import java.util.Objects;

public class User {
    private int databaseId;
    private String m_userId;
    private String m_username;
    private String m_password;

    public User() {
    }

    public User(int databaseId, String m_userId, String m_username, String m_password) {
        this.databaseId = databaseId;
        this.m_userId = m_userId;
        this.m_username = m_username;
        this.m_password = m_password;
    }

    public int getDatabaseId() {
        return databaseId;
    }

    public void setDatabaseId(int databaseId) {
        this.databaseId = databaseId;
    }

    public String getM_userId() {
        return m_userId;
    }

    public void setM_userId(String m_userId) {
        this.m_userId = m_userId;
    }

    public String getM_username() {
        return m_username;
    }

    public void setM_username(String m_username) {
        this.m_username = m_username;
    }

    public String getM_password() {
        return m_password;
    }

    public void setM_password(String m_password) {
        this.m_password = m_password;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        User user = (User) o;
        return databaseId == user.databaseId &&
                Objects.equals(m_userId, user.m_userId) &&
                Objects.equals(m_username, user.m_username) &&
                Objects.equals(m_password, user.m_password);
    }

    @Override
    public int hashCode() {
        return Objects.hash(databaseId, m_userId, m_username, m_password);
    }

    @Override
    public String toString() {
        return "User{" +
                "databaseId=" + databaseId +
                ", m_userId='" + m_userId + '\'' +
                ", m_username='" + m_username + '\'' +
                ", m_password='" + m_password + '\'' +
                '}';
    }
}
