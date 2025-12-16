using System.Collections.Generic;

namespace MapShortestPath 
{
   
    public class PlaceInfo
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Content { get; set; }
    }

    public static class LocationData
    {
        private static Dictionary<string, PlaceInfo> database = new Dictionary<string, PlaceInfo>();

        static LocationData()
        {
            // A - Dinh Độc Lập
            database["A"] = new PlaceInfo
            {
                Name = "Dinh Độc Lập",
                Address = "135 Nam Kỳ Khởi Nghĩa, Quận 1, TP.HCM",
                Content = "Di tích lịch sử quan trọng, nơi đánh dấu sự kiện 30/4/1975 thống nhất đất nước. Trước đây là nơi ở và làm việc của Tổng thống VNCH."
            };

            // B - Landmark 81
            database["B"] = new PlaceInfo
            {
                Name = "Landmark 81",
                Address = "KĐT Vinhomes Central Park, Bình Thạnh, TP.HCM",
                Content = "Tòa nhà cao nhất Việt Nam (461.3m), hoàn thành năm 2018. Biểu tượng của sự phát triển hiện đại và thịnh vượng."
            };

            // C - Địa đạo Củ Chi
            database["C"] = new PlaceInfo
            {
                Name = "Khu di tích Địa đạo Củ Chi",
                Address = "Xã Phú Mỹ Hưng, Huyện Củ Chi, TP.HCM",
                Content = "Hệ thống đường hầm dài 250km dưới lòng đất, biểu tượng cho ý chí kiên cường của quân dân Củ Chi trong chiến tranh."
            };

            // D - Rừng Sác Cần Giờ
            database["D"] = new PlaceInfo
            {
                Name = "Rừng Sác Cần Giờ",
                Address = "Huyện Cần Giờ, TP.HCM",
                Content = "Khu dự trữ sinh quyển thế giới. Chiến khu xưa của Đặc công Rừng Sác với hệ sinh thái rừng ngập mặn đa dạng."
            };

            // E - Bãi biển Cần Giờ
            database["E"] = new PlaceInfo
            {
                Name = "Bãi biển Cần Giờ",
                Address = "Thị trấn Cần Thạnh, Cần Giờ, TP.HCM",
                Content = "Bãi biển cát đen đặc trưng do phù sa, nơi lý tưởng để nghỉ dưỡng và thưởng thức hải sản tươi sống."
            };

            // F - Láng Le Bàu Cò
            database["F"] = new PlaceInfo
            {
                Name = "Khu di tích Láng Le Bàu Cò",
                Address = "Xã Lê Minh Xuân, Bình Chánh, TP.HCM",
                Content = "Nơi ghi dấu trận đánh lớn năm 1946 mở màn cho cuộc kháng chiến chống Pháp tại Sài Gòn - Chợ Lớn - Gia Định."
            };

            // G - Quảng trường Phước Hải
            database["G"] = new PlaceInfo
            {
                Name = "Quảng trường Phước Hải",
                Address = "Huyện Đất Đỏ, Bà Rịa - Vũng Tàu",
                Content = "Không gian công cộng ven biển, nơi người dân và du khách vui chơi, thả diều và ngắm hoàng hôn."
            };

            // H - Quảng trường Tân Uyên
            database["H"] = new PlaceInfo
            {
                Name = "Quảng trường Thành phố Tân Uyên",
                Address = "Phường Uyên Hưng, TP. Tân Uyên, Bình Dương",
                Content = "Trung tâm văn hóa, chính trị mới của thành phố, nơi tổ chức các sự kiện lớn và sinh hoạt cộng đồng."
            };

            // I - Quảng trường Dĩ An
            database["I"] = new PlaceInfo
            {
                Name = "Quảng trường Dĩ An",
                Address = "Trung tâm hành chính Dĩ An, Bình Dương",
                Content = "Nổi bật với cột cờ xoay 360 độ, không gian xanh hiện đại phục vụ người dân tập thể dục và vui chơi."
            };

            // J - Bãi Sau Vũng Tàu
            database["J"] = new PlaceInfo
            {
                Name = "Bãi Sau Vũng Tàu",
                Address = "Đường Thùy Vân, TP. Vũng Tàu",
                Content = "Bãi tắm nổi tiếng dài 5km với cát trắng mịn, nước trong xanh, là điểm đến du lịch biển hàng đầu."
            };
        }

        public static PlaceInfo Get(string id)
        {
            if (database.ContainsKey(id)) return database[id];

            return new PlaceInfo
            {
                Name = "Chưa có dữ liệu",
                Address = "...",
                Content = "Không tìm thấy thông tin cho địa điểm " + id
            };
        }
    }
}