import React from "react";

const AboutUsPage = () => {
  return (
    <div className="bg-gray-100 font-sans antialiased min-h-screen py-16 md:py-24">
      <div className="container mx-auto px-4 md:px-6 lg:px-8">
        <section className="mb-12 md:mb-16">
          <h1 className="text-3xl md:text-4xl lg:text-5xl font-bold text-gray-900 tracking-tight mb-4">
            About Our Library
          </h1>
          <p className="text-lg text-gray-600 leading-relaxed">
            Welcome to ABC Library, your gateway to a world of knowledge and
            stories. Our journey began in 2025 with a vision to create a
            welcoming and accessible space for learning, discovery, and
            community engagement through the power of books and resources. We
            believe in fostering a love of reading and providing opportunities
            for lifelong learning for everyone in Vietnam.
          </p>
        </section>

        <section className="mb-12 md:mb-16">
          <h2 className="text-2xl md:text-3xl font-semibold text-gray-900 tracking-tight mb-6">
            Our Core Values
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            <div className="bg-white rounded-lg p-6 shadow-sm">
              <h3 className="text-xl font-semibold text-gray-800 mb-2">
                Accessibility
              </h3>
              <p className="text-gray-600 leading-relaxed">
                We are committed to providing equal access to our collection and
                services for all members of our community, regardless of their
                background or abilities.
              </p>
            </div>
            <div className="bg-white rounded-lg p-6 shadow-sm">
              <h3 className="text-xl font-semibold text-gray-800 mb-2">
                Community
              </h3>
              <p className="text-gray-600 leading-relaxed">
                We strive to be a central hub for our community, offering
                programs, events, and spaces that encourage connection,
                collaboration, and shared learning experiences.
              </p>
            </div>
            <div className="bg-white rounded-lg p-6 shadow-sm">
              <h3 className="text-xl font-semibold text-gray-800 mb-2">
                Lifelong Learning
              </h3>
              <p className="text-gray-600 leading-relaxed">
                We believe in the power of continuous learning and provide
                resources and opportunities to support intellectual curiosity
                and personal growth at every stage of life.
              </p>
            </div>
          </div>
        </section>

        <section className="mb-12 md:mb-16">
          <h2 className="text-2xl md:text-3xl font-semibold text-gray-900 tracking-tight mb-6">
            Meet Our Team
          </h2>
          <p className="text-lg text-gray-600 leading-relaxed mb-6 text-center">
            Behind [Your Library Name] is a dedicated team passionate about
            connecting people with information and fostering a love of reading.
          </p>
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-8">
            <div className="bg-white rounded-lg p-4 shadow-sm text-center">
              <img
                src="https://via.placeholder.com/150"
                alt="Librarian 1"
                className="rounded-full w-24 h-24 mx-auto mb-3 object-cover"
              />
              <h4 className="font-semibold text-gray-800">
                [Librarian Name 1]
              </h4>
              <p className="text-sm text-gray-500">Head Librarian</p>
            </div>
            <div className="bg-white rounded-lg p-4 shadow-sm text-center">
              <img
                src="https://via.placeholder.com/150"
                alt="Librarian 2"
                className="rounded-full w-24 h-24 mx-auto mb-3 object-cover"
              />
              <h4 className="font-semibold text-gray-800">
                [Librarian Name 2]
              </h4>
              <p className="text-sm text-gray-500">Cataloging Specialist</p>
            </div>
            {/* Add more team members here */}
          </div>
          <p className="mt-6 text-gray-600 leading-relaxed text-center">
            Our team is here to assist you with your information needs and make
            your library experience enriching and enjoyable.
          </p>
        </section>

        <section className="mb-12 md:mb-16">
          <h2 className="text-2xl md:text-3xl font-semibold text-gray-900 tracking-tight mb-6">
            Our Vision for the Future
          </h2>
          <p className="text-lg text-gray-600 leading-relaxed">
            We envision ABC Library as a dynamic and evolving hub for knowledge,
            culture, and community engagement in the digital age. We are
            committed to expanding our digital resources, enhancing our online
            services, and creating innovative programs that meet the changing
            needs of our users. Our goal is to remain a vital resource that
            empowers individuals and strengthens our community for generations
            to come.
          </p>
        </section>

        <section className="text-center py-8">
          <h2 className="text-xl font-semibold text-gray-800 mb-4">
            Want to Get Involved?
          </h2>
          <a
            href="/contact" // Replace with your actual contact/support page link
            className="inline-block bg-blue-500 hover:bg-blue-700 text-white font-bold py-3 px-6 rounded-full transition duration-300"
          >
            Contact Us
          </a>
        </section>

        <footer className="py-8 text-center text-gray-500 text-sm">
          &copy; {new Date().getFullYear()} ABC Library. All rights reserved.
        </footer>
      </div>
    </div>
  );
};

export default AboutUsPage;
