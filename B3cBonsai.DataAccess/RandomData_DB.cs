using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3cBonsai.DataAccess
{
    //Random dữ liệu cho csdl
    internal class RandomData_DB
    {
        private static readonly Lazy<RandomData_DB> instance = new Lazy<RandomData_DB>(() => new RandomData_DB());
        public static RandomData_DB Instance = instance.Value;

        #region//Tên Cake
        List<string> _CakeName1 = new List<string>() { "Hoang", "Thiên", "Địa", "Nhân" };
        List<string> _CakeName2 = new List<string>() { "Thực", "Bảo", "Liên", "Mễ" };
        #endregion

        #region//Danh sách tên
        string[] listName1 = { "Trần", "Chí", "Trung", "Tráng", "Khánh", "Lương", "Trúc", "Anh", "Đài" };
        string[] listName2 = { "Giang", "Bát", "Ánh", "Sĩ", "Sơn", "Thực", "Vi", "Anh", "Loan" };
        string[] listName3 = { "Đế", "Quỵ", "Nguyệt", "Trai", "Sắc", "Phàm", "Vị", "Quái", "Bối" };
        #endregion

        #region//Tên hành tinh
        string[] planets = { "Mặt trăng", "Sao Thiên Vương", "Sao Hải Vương" };
        #endregion

        #region//Tên đơn vị tính
        List<string> _cakeDonViTinh = new List<string>() { "Tô", "Bát", "Đĩa", "1 phần", "1 bàn", "Tự do gọi" };
        #endregion

        #region//Danh sách ảnh thức ăn
        List<string> _linkFoodImage = new List<string>() { "https://i.pinimg.com/564x/29/2c/56/292c563994044c90473cd7cd4d58078a.jpg", "https://i.pinimg.com/736x/fb/5a/b0/fb5ab077e12b4c74181cc5f651e6e984.jpg", "https://i.pinimg.com/736x/0d/66/3c/0d663c7ff659fae2ab56ea135515a8d1.jpg", "https://i.pinimg.com/736x/0d/66/3c/0d663c7ff659fae2ab56ea135515a8d1.jpg", "https://i.pinimg.com/736x/f9/24/a1/f924a113b4e560fe5dcaff31716aa098.jpg", "https://i.pinimg.com/564x/f6/be/31/f6be31b5bfc8bb6f119f18dd8ca3a270.jpg" };
        #endregion

        #region//Danh sách ảnh bánh mì
        List<string> _breadImage = new List<string>() { "https://i.pinimg.com/736x/83/d4/1b/83d41b3be59a8f2b325891bf996e44e8.jpg", "https://i.pinimg.com/564x/7d/af/1e/7daf1e5c315b2cc805d0f80ca476fcf8.jpg", "https://i.pinimg.com/564x/c0/9f/39/c09f39a802af839ef5a3e60052177aa4.jpg", "https://i.pinimg.com/564x/0e/fc/23/0efc23915d063ee5f7881d3b04b350b0.jpg", "https://i.pinimg.com/564x/f6/c8/86/f6c8862d867fdee7dd5b194d27747cae.jpg", "https://i.pinimg.com/564x/23/1c/1a/231c1a4e9c8f2f1e2a4d18dc086cb8ae.jpg" };
        #endregion

        #region//Danh sách người dùng
        List<string> _userImage = new List<string>() { "https://i.pinimg.com/736x/c2/8d/d8/c28dd8b5e5ab6c5668c627e7129b5f9d.jpg", "https://i.pinimg.com/736x/9f/5d/25/9f5d254e9c42897b90dc86e82ba44bd9.jpg", "https://i.pinimg.com/736x/47/d6/03/47d6035cf8f8b957b1da35e8c6d93511.jpg", "https://i.pinimg.com/564x/e1/ff/68/e1ff68caa7eb40a5c903152f100e4fc3.jpg", "https://i.pinimg.com/736x/2b/d2/72/2bd2720fc597ae583f25c4ba9e0c48f9.jpg", "https://i.pinimg.com/564x/b8/a2/d1/b8a2d11155df433c8423dad1758868fd.jpg", "https://images.pexels.com/photos/3035875/pexels-photo-3035875.jpeg?auto=compress&cs=tinysrgb&w=600", "https://images.pexels.com/photos/5857355/pexels-photo-5857355.jpeg?auto=compress&cs=tinysrgb&w=600", "https://images.pexels.com/photos/28874283/pexels-photo-28874283/free-photo-of-khong-gian-lam-vi-c-t-i-gi-n-v-i-may-tinh-xach-tay-va-c-c-ca-phe.jpeg?auto=compress&cs=tinysrgb&w=600&lazy=load", "https://images.pexels.com/photos/28494944/pexels-photo-28494944.jpeg?auto=compress&cs=tinysrgb&w=600&lazy=load", "https://images.pexels.com/photos/28570315/pexels-photo-28570315/free-photo-of-ng-i-ph-n-tr-v-i-may-tinh-b-ng-va-tai-nghe.jpeg?auto=compress&cs=tinysrgb&w=600&lazy=load", "https://i.pinimg.com/control/564x/b2/9a/90/b29a904f241d59034715eaf44eaad426.jpg" };
        #endregion

        #region//Danh sách ảnh chi nhánh
        List<string> _linkBranchImage = new List<string>() { "https://i.pinimg.com/564x/55/08/e1/5508e15188aff6e7d287399d1395bd56.jpg", "https://i.pinimg.com/564x/8f/88/c2/8f88c2018a75f40ef7a14016d1890d75.jpg", "https://i.pinimg.com/564x/00/7d/b4/007db4a042d296c457ef18af3b095e78.jpg", "https://i.pinimg.com/564x/95/10/8d/95108d00785a290e4d73cfe67094ec30.jpg", "https://i.pinimg.com/564x/c5/f8/f8/c5f8f8da8950a2004e40ce9ea4051116.jpg", "https://i.pinimg.com/564x/6c/cd/31/6ccd315173ea7a1ffa7d0849b526d5bc.jpg" };
        #endregion

        #region//Chuỗi chữ cái
        string _stringLetter = "qwertyuiopasdfghjklzxcvbnm1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
        #endregion

        #region//Đuôi email
        string[] domains = { "@gmail.com", "@yahoo.com", "@outlook.com", "@example.com" };
        #endregion

        #region//Mô tả cake
        string[] descript = { "Món này ngon nhưng nhiều mỡ", "Ăn nhiều dễ béo", "Ăn ít thôi" };
        #endregion

        #region//Random video Bonsai
        string[] linkVideos = { "https://videos.pexels.com/video-files/853839/853839-sd_640_360_30fps.mp4", "https://videos.pexels.com/video-files/3132090/3132090-sd_640_360_24fps.mp4", "https://videos.pexels.com/video-files/856030/856030-sd_640_360_25fps.mp4", "https://videos.pexels.com/video-files/3176704/3176704-sd_360_640_30fps.mp4", "https://videos.pexels.com/video-files/28967565/12531601_640_360_60fps.mp4", "https://videos.pexels.com/video-files/1494279/1494279-sd_640_360_24fps.mp4", "https://videos.pexels.com/video-files/4947342/4947342-sd_640_360_30fps.mp4" };
        #endregion

        #region//Random image Bonsai
        string[] linkImgBonsai = { "https://i.pinimg.com/736x/39/0c/0a/390c0a43f21becd7305ad73a48760f12.jpg", "https://i.pinimg.com/564x/0b/7d/ae/0b7dae3946327c7b03726c0f41ae1def.jpg", "https://i.pinimg.com/564x/4f/97/20/4f9720e73caa7c1a276df615457188c7.jpg", "https://images.pexels.com/photos/1382195/pexels-photo-1382195.jpeg?auto=compress&cs=tinysrgb&w=600", "https://images.pexels.com/photos/2149105/pexels-photo-2149105.jpeg?auto=compress&cs=tinysrgb&w=600", "https://images.pexels.com/photos/6072061/pexels-photo-6072061.jpeg?auto=compress&cs=tinysrgb&w=600", "https://images.pexels.com/photos/4050790/pexels-photo-4050790.jpeg?auto=compress&cs=tinysrgb&w=600", "https://images.pexels.com/photos/5765694/pexels-photo-5765694.jpeg?auto=compress&cs=tinysrgb&w=600", "https://images.pexels.com/photos/5831008/pexels-photo-5831008.jpeg?auto=compress&cs=tinysrgb&w=600", "https://images.pexels.com/photos/28822467/pexels-photo-28822467/free-photo-of-cay-bonsai-phong-mua-thu-r-c-r-tren-n-n-t-i.jpeg?auto=compress&cs=tinysrgb&w=600", "https://images.pexels.com/photos/6852309/pexels-photo-6852309.jpeg?auto=compress&cs=tinysrgb&w=600" };
        #endregion


        #region//Hinh ảnh combo Bonsai
        string[] imagesComboBonsai = new string[] { "https://cdn.pixabay.com/photo/2016/02/10/21/59/landscape-1192669_640.jpg", "https://cdn.pixabay.com/photo/2023/10/12/07/34/mountain-8310076_640.jpg", "https://cdn.pixabay.com/photo/2016/10/19/08/57/mountains-1752433_640.jpg", "https://cdn.pixabay.com/photo/2016/08/12/23/44/river-1590010_640.jpg", "https://cdn.pixabay.com/photo/2022/05/23/11/26/tree-7215935_640.jpg", "https://cdn.pixabay.com/photo/2023/06/16/21/13/landscape-8068792_640.jpg" };
        #endregion
        Random rd = new Random();
        public string _CakeName()
        {
            return String.Join(" ", new string[] { _CakeName1[rd.Next(0, _CakeName1.Count)], _CakeName2[rd.Next(0, _CakeName2.Count)] });
        }
        public string _CakeDonViTinh()
        {
            return _cakeDonViTinh[rd.Next(0, _cakeDonViTinh.Count)];
        }

        public string _CakeImage()
        {
            return _linkFoodImage[rd.Next(0, _linkFoodImage.Count)];
        }

        public string _BranchImage()
        {
            return _linkBranchImage[rd.Next(_linkBranchImage.Count)];
        }

        public string _UserImage()
        {
            return _userImage[rd.Next(_userImage.Count)];
        }
        public string rdString()
        {
            return new string(new char[10].Select(C => _stringLetter[rd.Next(0, _stringLetter.Length)]).ToArray());
        }

        public string rdName()
        {
            return $"{listName1[rd.Next(0, listName1.Length)]} {listName2[rd.Next(0, listName2.Length)]} {listName3[rd.Next(0, listName3.Length)]}";
        }

        public string rdAddress()
        {
            return $"{planets[rd.Next(0, planets.Length)]}, tọa độ: {rd.Next(-90, 90)} Kinh độ, {rd.Next(-180, 180)} Vĩ độ";
        }

        public string _Email()
        {
            return rdString() + domains[rd.Next(0, domains.Length)];

        }

        public string _Descript()
        {
            return descript[rd.Next(0, descript.Length)];
        }

        public string _BreadImage()
        {
            return _breadImage[rd.Next(_breadImage.Count)];
        }

        #region CS4

        // Random danh mục sản phẩm
        public string RandomCategoryName()
        {
            string[] categories = { "Cây lá màu", "Cây thân gỗ bonsai", "Cây hoa cảnh",
                                "Cây xương rồng và cây mọng nước", "Cây cảnh để bàn",
                                "Cây cảnh thủy sinh", "Cây phong thủy", "Cây leo và cây treo" };
            return categories[rd.Next(categories.Length)];
        }

        // Random mô tả danh mục
        public string RandomCategoryDescription()
        {
            string[] descriptions = { "Trang trí nội thất", "Độ bền cao", "Dễ chăm sóc",
                                  "Phù hợp văn phòng", "Trang trí ban công",
                                  "Tăng vượng khí", "Thu hút tài lộc", "Làm sạch không khí" };
            return descriptions[rd.Next(descriptions.Length)];
        }

        // Random tên sản phẩm
        public string RandomProductName()
        {
            string[] products = { "Cây Lan Ý", "Cây Kim Tiền", "Cây Lưỡi Hổ", "Cây Trầu Bà",
                              "Cây Sen Đá", "Cây Xương Rồng", "Cây Tùng La Hán",
                              "Cây Cọ Nhật", "Cây Đuôi Công" };
            return products[rd.Next(products.Length)];
        }

        // Random mô tả sản phẩm
        public string RandomProductDescription()
        {
            string[] descriptions = { "Cây dễ chăm sóc", "Cây thu hút tài lộc", "Lá đẹp, làm sạch không khí",
                                  "Cây chịu bóng tốt", "Trang trí nội thất và bàn làm việc" };
            return descriptions[rd.Next(descriptions.Length)];
        }

        // Random ảnh sản phẩm
        public string RandomProductImage()
        {
            string[] imageUrls = { "https://example.com/image1.jpg", "https://example.com/image2.jpg",
                               "https://example.com/image3.jpg", "https://example.com/image4.jpg" };
            return imageUrls[rd.Next(imageUrls.Length)];
        }

        // Random ngày thuê nhân viên
        public DateTime RandomHiredDate()
        {
            return DateTime.Now.AddDays(-rd.Next(365, 1825)); // Từ 1 đến 5 năm trước
        }
        // Random số điện thoại
        public string RandomPhone()
        {
            return "09" + rd.Next(10000000, 99999999).ToString();
        }
        // Random video
        public string RandomVideoBonsai()
        {
            return linkVideos[rd.Next(linkVideos.Length)];
        }
        // Random image bonsai
        public string RandomImageBonsai()
        {
            return linkImgBonsai[rd.Next(linkImgBonsai.Length)];
        }

        // Random image combo bonsai
        public string RandomImageComboBonsai()
        {
            return imagesComboBonsai[rd.Next(imagesComboBonsai.Length)];
        }
        #endregion
    }
}
