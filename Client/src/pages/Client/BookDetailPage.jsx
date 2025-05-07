import { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import axios from "axios";
import StarRating from "../../components/StarRating";
import { useAuth } from "../../components/Header";
const BookDetailsPage = () => {
  const { authAxios } = useAuth();
  const { id } = useParams();
  const [book, setBook] = useState(null);
  const [reviews, setReviews] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [userReview, setUserReview] = useState({
    rating: 0,
    comment: "",
  });
  const API_URL = import.meta.env.VITE_API_URL;
  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const [bookRes, reviewsRes] = await Promise.all([
          axios.get(`${API_URL}/books/${id}`),
          axios.get(`${API_URL}/bookReview/withBook/${id}`),
        ]);
        setBook(bookRes.data);
        const reviewsData = reviewsRes.data || [];
        setReviews(Array.isArray(reviewsData) ? reviewsData : [reviewsData]);
      } catch (err) {
        setError(err.response?.data?.message || "Failed to fetch data");
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [id]);

  const handleSubmitReview = async (e) => {
    e.preventDefault();
    try {
      const response = await authAxios.post(`/bookReview/${id}`, {
        rating: userReview.rating,
        comment: userReview.comment,
      });
      setReviews([...reviews, response.data]);
      setUserReview({ rating: 0, comment: "" });
    } catch (err) {
      setError(err.response?.data?.message || "Failed to submit review");
    }
  };
  const getMedianRating = (reviews) => {
    const ratings = reviews
      .map((r) => r.rating)
      .filter((r) => typeof r === "number" && !isNaN(r));

    if (ratings.length === 0) return 0;

    ratings.sort((a, b) => a - b);
    const mid = Math.floor(ratings.length / 2);

    const rawMedian =
      ratings.length % 2 !== 0
        ? ratings[mid]
        : (ratings[mid - 1] + ratings[mid]) / 2;

    return Math.round(rawMedian * 2) / 2;
  };

  if (loading)
    return (
      <div className="flex justify-center items-center h-screen">
        Loading...
      </div>
    );
  if (error)
    return <div className="text-red-500 text-center mt-8">{error}</div>;
  if (!book) return <div className="text-center mt-8">Book not found</div>;

  return (
    <div className="max-w-6xl mx-auto px-4 py-8">
      {/* Book Details Section */}
      <div className="flex flex-col md:flex-row gap-8 mb-12">
        <div className="w-full md:w-1/3 lg:w-1/4">
          <img
            src={book.imageUrl}
            alt={book.name}
            className="w-full h-auto rounded-lg shadow-md object-cover aspect-[2/3]"
          />
        </div>

        <div className="w-full md:w-2/3 lg:w-3/4">
          <h1 className="text-3xl font-bold text-gray-900 mb-2">{book.name}</h1>
          <p className="text-xl text-gray-600 mb-4">by {book.author}</p>

          <div className="mb-6">
            <span className="inline-block bg-gray-200 rounded-full px-3 py-1 text-sm font-semibold text-gray-700">
              {book.categoryName}
            </span>
          </div>

          <div className="flex items-center mb-6">
            <StarRating rating={getMedianRating(reviews) || 0} />
            <span className="ml-2 text-gray-600">
              ({reviews.length} {reviews.length === 1 ? "review" : "reviews"})
            </span>
          </div>

          <p className="text-gray-700 mb-6">{book.description}</p>
        </div>
      </div>

      {/* Reviews Section */}
      <div className="border-t border-gray-200 pt-8">
        <h2 className="text-2xl font-bold text-gray-900 mb-6">
          Customer Reviews
        </h2>

        {reviews.length === 0 ? (
          <p className="text-gray-500">
            No reviews yet. Be the first to review!
          </p>
        ) : (
          <div className="space-y-6">
            {reviews.map((review) => (
              <div
                key={review.id || review._id}
                className="border-b border-gray-100 pb-6 last:border-0"
              >
                <div className="flex items-center mb-2">
                  <StarRating rating={review.rating} />
                  <span className="ml-2 text-sm text-gray-500">
                    {new Date(
                      review.createdAt || review.reviewDate
                    ).toLocaleDateString()}
                  </span>
                </div>
                <h3 className="font-medium text-gray-900">
                  {review.reviewerName || review.user?.name || "Anonymous"}
                </h3>
                <p className="text-gray-700 mt-1">{review.comment}</p>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Add Review Section */}
      <div className="mt-12 border-t border-gray-200 pt-8">
        <h2 className="text-2xl font-bold text-gray-900 mb-6">
          Write a Review
        </h2>
        <form onSubmit={handleSubmitReview} className="max-w-2xl">
          <div className="mb-4">
            <label className="block text-gray-700 mb-2">Your Rating</label>
            <StarRating
              editable={true}
              rating={userReview.rating}
              onRatingChange={(rating) =>
                setUserReview({ ...userReview, rating })
              }
            />
          </div>

          <div className="mb-4">
            <label htmlFor="comment" className="block text-gray-700 mb-2">
              Your Review
            </label>
            <textarea
              id="comment"
              rows="4"
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              value={userReview.comment}
              onChange={(e) =>
                setUserReview({ ...userReview, comment: e.target.value })
              }
              required
            ></textarea>
          </div>

          <button
            type="submit"
            className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors"
          >
            Submit Review
          </button>
        </form>
      </div>
    </div>
  );
};

export default BookDetailsPage;
