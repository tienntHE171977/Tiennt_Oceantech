create database Oceantech;
use Oceantech

-- Tạo bảng DanhMucTinh
CREATE TABLE DanhMucTinh (
    TinhID INT PRIMARY KEY IDENTITY(1,1),
    TenTinh NVARCHAR(100) NOT NULL
);

-- Tạo bảng DanhMucHuyen
CREATE TABLE DanhMucHuyen (
    HuyenID INT PRIMARY KEY IDENTITY(1,1),
    TenHuyen NVARCHAR(100) NOT NULL,
    TinhID INT FOREIGN KEY REFERENCES DanhMucTinh(TinhID)
);

-- Tạo bảng DanhMucXa
CREATE TABLE DanhMucXa (
    XaID INT PRIMARY KEY IDENTITY(1,1),
    TenXa NVARCHAR(100) NOT NULL,
    HuyenID INT FOREIGN KEY REFERENCES DanhMucHuyen(HuyenID)
);

-- Tạo bảng DanToc
CREATE TABLE DanToc (
    DanTocID INT PRIMARY KEY IDENTITY(1,1),
    TenDanToc NVARCHAR(100) NOT NULL
);

-- Tạo bảng NgheNghiep
CREATE TABLE NgheNghiep (
    NgheNghiepID INT PRIMARY KEY IDENTITY(1,1),
    TenNgheNghiep NVARCHAR(100) NOT NULL
);

-- Tạo bảng Employee
CREATE TABLE Employee (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE,
    Tuoi INT,
    DanTocID INT FOREIGN KEY REFERENCES DanToc(DanTocID),
    NgheNghiepID INT FOREIGN KEY REFERENCES NgheNghiep(NgheNghiepID),
    CCCD NVARCHAR(20),
    SoDienThoai NVARCHAR(15),
    TinhID INT FOREIGN KEY REFERENCES DanhMucTinh(TinhID),
    HuyenID INT FOREIGN KEY REFERENCES DanhMucHuyen(HuyenID),
    XaID INT FOREIGN KEY REFERENCES DanhMucXa(XaID),
    DiaChiCuThe NVARCHAR(255)
);
