import { useState } from "react";

const StarRating = ({ rating, editable = false, onRatingChange }) => {
  const [hoverRating, setHoverRating] = useState(0);

  const handleClick = (newRating) => {
    if (editable && onRatingChange) {
      onRatingChange(newRating);
    }
  };

  const handleMouseEnter = (newHoverRating) => {
    if (editable) {
      setHoverRating(newHoverRating);
    }
  };

  const handleMouseLeave = () => {
    if (editable) {
      setHoverRating(0);
    }
  };

  return (
    <div className="flex">
      {[1, 2, 3, 4, 5].map((star) => {
        const filled = hoverRating ? star <= hoverRating : star <= rating;
        return (
          <button
            key={star}
            type={editable ? "button" : "div"}
            className={`text-2xl ${
              filled ? "text-yellow-400" : "text-gray-300"
            } ${editable ? "cursor-pointer" : "cursor-default"}`}
            onClick={() => handleClick(star)}
            onMouseEnter={() => handleMouseEnter(star)}
            onMouseLeave={handleMouseLeave}
            disabled={!editable}
          >
            ★
          </button>
        );
      })}
    </div>
  );
};

export default StarRating;
